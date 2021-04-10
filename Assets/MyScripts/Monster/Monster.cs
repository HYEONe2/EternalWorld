using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    private enum STATE { STATE_IDLE, STATE_PATROL, STATE_TARGET, STATE_BATTLE, STATE_END };

    // MyComponents
    private NavMeshAgent m_Agent;
    private Animator m_Animator;

    // Others
    private MazeManager m_MazeManager;
    private GameObject m_Target;

    private GameObject[] m_SkillObject = new GameObject[2];
    private GameObject m_FirstSkill;
    private GameObject m_SecondSkill;

    // Values
    [SerializeField] private ObjectManager.ABILITY m_eAbility = ObjectManager.ABILITY.ABIL_END;
    private STATE m_eState;

    private bool[] m_bSkillOn = new bool[2];
    private float[] m_CoolTime = new float[2];

    private bool m_bPatrolMonster;
    private Vector3 m_PatrolPos;

    private int m_HP;

    // Animator Values
    private readonly int m_bHashTargetAround = Animator.StringToHash("IsTargetAround");
    private readonly int m_bHashOnBattle = Animator.StringToHash("OnBattle");
    private readonly int m_HashTargetDist = Animator.StringToHash("TargetDist");
    private readonly int m_bHashUseSkill = Animator.StringToHash("UseSkill");
    private readonly int m_HashSkillIndex = Animator.StringToHash("SkillIndex");
    private readonly int m_bHashDamaged = Animator.StringToHash("IsDamaged");

    // Start is called before the first frame update
    void Start()
    {
        InitializeComponents();
        InitializeValues();
    }

    // Update is called once per frame
    void Update()
    {
        CheckStateByTargetDist();
        UpdateState();
    }

    private void OnDestroy()
    {
        foreach (GameObject skill in m_SkillObject)
            Destroy(skill);
    }

    private void InitializeComponents()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();

        m_MazeManager = GameObject.Find("MazeManager").GetComponent<MazeManager>();
        m_Target = GameObject.Find("Player");
    }

    private void InitializeValues()
    {
        m_bPatrolMonster = (Random.Range(0, 2) == 1) ? true : false;
        m_Animator.SetBool("IsPatrolMon", m_bPatrolMonster);

        if (m_bPatrolMonster)
        {
            m_eState = STATE.STATE_PATROL;
            m_PatrolPos = m_MazeManager.GetPatrolPos();
        }
        else
            m_eState = STATE.STATE_IDLE;

        switch (m_eAbility)
        {
            case ObjectManager.ABILITY.ABIL_FIRE:
                {
                    m_FirstSkill = null;
                    m_SecondSkill = Resources.Load<GameObject>("Particle/Monster/Fire/FlameThrower");
                }
                break;
            case ObjectManager.ABILITY.ABIL_GRASS:
                {

                }
                break;
            case ObjectManager.ABILITY.ABIL_WATER:
                {

                }
                break;
        }

        m_HP = 2;
    }

    private void CheckStateByTargetDist()
    {
        float Distance = (m_Target.transform.position - transform.position).magnitude;
        m_Animator.SetFloat(m_HashTargetDist, Distance);

        if (Distance < 20f && Distance > 5f)
            SetState(STATE.STATE_TARGET);
        else if (Distance <= 5f)
            SetState(STATE.STATE_BATTLE);
        else
        {
            if (m_bPatrolMonster)
                SetState(STATE.STATE_PATROL);
            else
                SetState(STATE.STATE_IDLE);
        }
    }

    private void UpdateState()
    {
        switch (m_eState)
        {
            case STATE.STATE_PATROL:
                {
                    if (CheckPatrolDistance())
                        m_PatrolPos = m_MazeManager.GetPatrolPos();
                    else
                        m_Agent.destination = m_PatrolPos;
                    break;
                }
            case STATE.STATE_TARGET:
                {
                    Vector3 NewLook = m_Target.transform.position - transform.position;
                    transform.rotation = Quaternion.LookRotation(NewLook);
                    m_Agent.destination = m_Target.transform.position;
                }
                break;
            case STATE.STATE_BATTLE:
                {
                    if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                        m_Animator.SetBool(m_bHashUseSkill, false);

                    UpdateSkill();
                }
                break;
        }
    }

    private void SetState(STATE state)
    {
        if (m_eState == state)
            return;

        // 애니메이션 같이 한 번만 설정 바꿔주면 되는 내용 넣어주기
        switch (state)
        {
            case STATE.STATE_IDLE:
                {
                    if (m_bPatrolMonster)
                        return;

                    m_Animator.SetBool(m_bHashTargetAround, false);
                    m_Animator.SetBool(m_bHashOnBattle, false);

                    m_Agent.isStopped = true;
                }
                break;
            case STATE.STATE_PATROL:
                {
                    if (!m_bPatrolMonster)
                        return;

                    m_Animator.SetBool(m_bHashTargetAround, false);
                    m_Animator.SetBool(m_bHashOnBattle, false);

                    m_Agent.isStopped = false;
                    m_Agent.speed = 5f;
                }
                break;
            case STATE.STATE_TARGET:
                {
                    ResetSKill();

                    m_Animator.SetBool(m_bHashTargetAround, true);
                    m_Animator.SetBool(m_bHashOnBattle, false);

                    m_Agent.isStopped = false;
                    m_Agent.speed = 8f;
                }
                break;
            case STATE.STATE_BATTLE:
                {
                    m_Animator.SetBool(m_bHashTargetAround, true);
                    m_Animator.SetBool(m_bHashOnBattle, true);

                    m_Agent.isStopped = true;
                    m_Agent.speed = 5f;
                }
                break;
        }

        m_eState = state;
    }

    private bool CheckPatrolDistance()
    {
        float Distance = (m_PatrolPos - transform.position).magnitude;

        if (Distance < 5f)
            return true;
        else
            return false;
    }

    private void UpdateSkill()
    {
        if (!UpdateCoolTime())
            return;

        //int SkillIndex = Random.Range(0, 2);
        int SkillIndex = 1;
        if (SkillIndex == 0)
            UseFirstSkill();
        else
            UseSecondSkill();
    }

    private bool UpdateCoolTime()
    {
        if (m_bSkillOn[0])
        {
            if (m_CoolTime[0] > 3f)
            {
                m_bSkillOn[0] = false;
                m_CoolTime[0] = 0;
                return true;
            }
            else
            {
                m_CoolTime[0] += Time.deltaTime;
                return false;
            }
        }

        if (m_bSkillOn[1])
        {
            if (m_CoolTime[1] > 5f)
            {
                m_bSkillOn[1] = false;
                m_CoolTime[1] = 0;
                return true;
            }
            else
            {
                m_CoolTime[1] += Time.deltaTime;
                return false;
            }
        }

        return true;
    }

    private void UseFirstSkill()
    {
        if (m_bSkillOn[0] || (m_eAbility != ObjectManager.ABILITY.ABIL_FIRE && m_SkillObject[0]))
            return;

        m_Animator.SetBool(m_bHashUseSkill, true);
        m_Animator.SetInteger(m_HashSkillIndex, 0);

        switch (m_eAbility)
        {
            case ObjectManager.ABILITY.ABIL_FIRE:
                {
                    m_Animator.speed = 0.7f;
                }
                break;
            case ObjectManager.ABILITY.ABIL_GRASS:
                {

                }
                break;
            case ObjectManager.ABILITY.ABIL_WATER:
                {

                }
                break;
        }

        m_bSkillOn[0] = true;
    }

    private void UseSecondSkill()
    {
        if (m_bSkillOn[1] || m_SkillObject[1])
            return;

        m_Animator.SetBool(m_bHashUseSkill, true);
        m_Animator.SetInteger(m_HashSkillIndex, 1);

        Vector3 Look = transform.forward;
        Vector3 NewPos;
        switch (m_eAbility)
        {
            case ObjectManager.ABILITY.ABIL_FIRE:
                {
                }
                break;
            case ObjectManager.ABILITY.ABIL_GRASS:
                {

                }
                break;
            case ObjectManager.ABILITY.ABIL_WATER:
                {

                }
                break;
        }

        m_bSkillOn[1] = true;
    }

    private void ResetSKill()
    {
        for (int i = 0; i < 2; ++i)
        {
            m_bSkillOn[i] = false;
            m_CoolTime[i] = 0;
        }
    }

    public void UseFireSkill()
    {
        m_Agent.isStopped = true;
        m_Animator.speed = 0.1f;

        Vector3 Look = transform.forward;
        Vector3 NewPos = transform.position + Look;
        NewPos.y += 1.7f;

        m_SkillObject[1] = Instantiate(m_SecondSkill, NewPos, Quaternion.identity);
        m_SkillObject[1].transform.rotation = Quaternion.LookRotation(Look);
    }

    public void EndFireSkill()
    {
        m_Agent.isStopped = false;
        m_Animator.speed = 1f;

        Destroy(m_SkillObject[1]);
        m_SkillObject[1] = null;
    }

    public void SetDamaged(int damage)
    {
        m_Agent.isStopped = true;
        m_Animator.speed = 0.5f;

        m_Animator.SetBool(m_bHashDamaged, true);
        m_Animator.SetBool(m_bHashUseSkill, false);

        for (int i = 0; i < 2; ++i)
            m_SkillObject[i] = null;

        m_HP -= damage;
        
        //if (m_HP <= 0)
        //{
        //이펙트 생성
        //    Destroy(gameObject);
        //    return;
        //}
    }

    public void ResetNormal()
    {
        m_Agent.isStopped = true;
        m_Animator.SetBool(m_bHashDamaged, false);
    }
}
