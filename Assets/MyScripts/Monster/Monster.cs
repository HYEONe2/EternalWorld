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
    private float m_TargetY;

    private GameObject[] m_SkillObject = new GameObject[2];
    private GameObject m_FirstSkill;
    private GameObject m_SecondSkill;

    // Values
    [SerializeField] private ObjectManager.ABILITY m_eAbility = ObjectManager.ABILITY.ABIL_END;
    private STATE m_eState;

    private bool[] m_bSkillOn = new bool[2];
    private float[] m_CoolTime = new float[2];
    private int m_SkillIndex;

    private bool m_bPatrolMonster;
    private Vector3 m_PatrolPos;

    private int m_HP;
    private bool m_bAttack;

    private bool m_bDamaged;
    private float m_DamageTime;

    private Material m_Material;
    private Color m_OriginColor;

    // Animator Values
    private readonly int m_bHashTargetAround = Animator.StringToHash("IsTargetAround");
    private readonly int m_bHashOnBattle = Animator.StringToHash("OnBattle");
    private readonly int m_HashTargetDist = Animator.StringToHash("TargetDist");
    private readonly int m_bHashUseSkill = Animator.StringToHash("UseSkill");
    private readonly int m_HashSkillIndex = Animator.StringToHash("SkillIndex");
    private readonly int m_bHashDamaged = Animator.StringToHash("IsDamaged");

    public bool GetAttack() { return m_bAttack; }
    public bool GetDamaged() { return m_Animator.GetBool(m_bHashDamaged); }
    public ObjectManager.ABILITY GetAbility() { return m_eAbility; }

    // Start is called before the first frame update
    void Start()
    {
        InitializeComponents();
        InitializeValues();
    }

    private void OnDestroy()
    {
        foreach (GameObject skill in m_SkillObject)
            Destroy(skill);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_HP <= 0)
            return;

        if(m_bDamaged)
        {
            if (m_DamageTime > 2f)
            {
                ObjectManager.SetOriginOpaque(m_Material, m_OriginColor);
                m_Animator.SetBool(m_bHashDamaged,false);
                m_DamageTime = 0;
                m_bDamaged = false;
            }
            else
            {
                ObjectManager.SetPingPongTransparent(m_Material, 1f, 0, 0, 1f);
                m_DamageTime += Time.deltaTime;
            }
        }

        CheckStateByTargetDist();
        UpdateState();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_Animator.GetBool(m_bHashDamaged))
            return;

        if (other.CompareTag("Weapon"))
        {
            if (m_Target.GetComponent<Player>().GetAttack())
            {
                // 파티클 추가! 스파크 튀는 파티클!!
                m_Agent.isStopped = true;
                m_Animator.SetBool(m_bHashDamaged, true);
                SetDamaged(m_Target.GetComponent<PlayerProperty>().GetPlayerStat().m_Str);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            m_Agent.isStopped = false;
        }
    }

    private void InitializeComponents()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();

        m_MazeManager = GameObject.Find("MazeManager").GetComponent<MazeManager>();
        m_Target = GameObject.Find("Player");
        m_TargetY = m_Target.transform.position.y;
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
                    m_FirstSkill = Resources.Load<GameObject>("Particle/Monster/Grass/PoisonAttack");
                    m_SecondSkill = transform.Find("RazorAttack").gameObject;
                    m_SecondSkill.SetActive(false);
                }
                break;
            case ObjectManager.ABILITY.ABIL_WATER:
                {
                    m_SecondSkill = Resources.Load<GameObject>("Particle/Monster/Water/BallAttack");
                }
                break;
        }

        m_HP = 10 * m_Target.GetComponent<PlayerProperty>().GetPlayerStat().m_Level;
        m_SkillIndex = Random.Range(0, 2);

        m_bDamaged = false;
        m_DamageTime = 0;

        m_Material = transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material;
        m_OriginColor = m_Material.color;
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
                    Vector3 TargetPos = m_Target.transform.position;
                    TargetPos.y = -3.169801f;

                    Vector3 NewLook = TargetPos - transform.position;
                    transform.rotation = Quaternion.LookRotation(NewLook);
                    m_Agent.destination = m_Target.transform.position;
                }
                break;
            case STATE.STATE_BATTLE:
                {
                    if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                        m_Animator.SetBool(m_bHashUseSkill, false);

                    UpdateSkill();

                    if (m_eAbility == ObjectManager.ABILITY.ABIL_WATER && m_SkillObject[1])
                        return;

                    Vector3 TargetPos = m_Target.transform.position;
                    TargetPos.y = -3.169801f;
                    Vector3 NewLook = TargetPos - transform.position;
                    transform.rotation = Quaternion.LookRotation(NewLook);
                }
                break;
        }
    }

    private void SetState(STATE state)
    {
        if (m_eState == state || m_Animator.GetBool(m_bHashDamaged))
        {
            for (int i = 0; i < 2; ++i)
            {
                //Destroy(m_SkillObject[i]);
                m_SkillObject[i] = null;
            }

            return;
        }

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
                    ResetSkill();

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
        if (m_Animator.GetBool(m_bHashDamaged))
            return;

        if (m_SkillIndex == 0)
            UseFirstSkill();
        else
            UseSecondSkill();
    }

    private bool UpdateCoolTime(int skillIndex)
    {
        if (m_bSkillOn[skillIndex])
        {
            if (m_CoolTime[skillIndex] > 8f)
            {
                m_SkillIndex = Random.Range(0, 2);
                m_bSkillOn[skillIndex] = false;
                m_CoolTime[skillIndex] = 0;

                return true;
            }
            else
            {
                m_CoolTime[skillIndex] += Time.deltaTime;

                return false;
            }
        }

        return true;
    }

    private void UseFirstSkill()
    {
        if (!UpdateCoolTime(0) || m_bSkillOn[0])
            return;

        m_Animator.SetBool(m_bHashUseSkill, true);
        m_Animator.SetInteger(m_HashSkillIndex, 0);

        Vector3 Look = transform.forward;
        Vector3 NewPos;

        switch (m_eAbility)
        {
            case ObjectManager.ABILITY.ABIL_FIRE:
                {
                    m_Animator.speed = 0.7f;
                }
                break;
            case ObjectManager.ABILITY.ABIL_GRASS:
                {
                    if (m_SkillObject[0] || m_SecondSkill.activeSelf)
                        return;

                    NewPos = transform.position + Look;
                    NewPos.y += 1.5f;

                    m_SkillObject[0] = Instantiate(m_FirstSkill, NewPos, Quaternion.identity);
                    m_SkillObject[0].transform.parent = transform;
                }
                break;
            case ObjectManager.ABILITY.ABIL_WATER:
                {
                    transform.Find("Body").GetComponent<BoxCollider>().isTrigger = true;
                }
                break;
        }

        m_bSkillOn[0] = true;
        m_bAttack = true;
    }

    private void UseSecondSkill()
    {
        if (!UpdateCoolTime(1) || m_bSkillOn[1])
            return;

        m_Animator.SetBool(m_bHashUseSkill, true);
        m_Animator.SetInteger(m_HashSkillIndex, 1);

        Vector3 Look = transform.forward;
        Vector3 NewPos;
        switch (m_eAbility)
        {
            case ObjectManager.ABILITY.ABIL_GRASS:
                {
                    m_Animator.speed = 0.1f;

                    m_SecondSkill.SetActive(true);
                }
                break;
            case ObjectManager.ABILITY.ABIL_WATER:
                {
                    if (m_SkillObject[1])
                        return;

                    NewPos = transform.position + Look * 3f;
                    NewPos.y += 1.5f;

                    m_SkillObject[1] = Instantiate(m_SecondSkill, NewPos, Quaternion.identity);
                    for (int i = 0; i < m_SkillObject[1].transform.childCount; ++i)
                        m_SkillObject[1].transform.GetChild(i).GetComponent<BallAttack>().SetLookVector(Look);
                }
                break;
        }

        m_bSkillOn[1] = true;
        m_bAttack = true;
    }

    private void ResetSkill()
    {
        for (int i = 0; i < 2; ++i)
        {
            m_bSkillOn[i] = false;
            m_CoolTime[i] = 7f;
        }
    }

    // FIRE
    public void UseFireSkill()
    {
        m_bAttack = true;
        m_Agent.isStopped = true;
        m_Animator.speed = 0.1f;

        Vector3 Look = transform.forward;
        Vector3 NewPos = transform.position + Look;
        NewPos.y += 1.7f;

        m_SkillObject[1] = Instantiate(m_SecondSkill, NewPos, Quaternion.identity);
        m_SkillObject[1].transform.rotation = Quaternion.LookRotation(Look);
        m_SkillObject[1].transform.parent = transform;
    }

    public void EndFireSkill()
    {
        m_bAttack = false;
        m_Agent.isStopped = false;

        m_Animator.speed = 1f;
        m_Animator.SetBool(m_bHashUseSkill, false);

        Destroy(m_SkillObject[1]);
        m_SkillObject[1] = null;
    }
    //

    public void SetDamaged(int damage)
    {
        m_bAttack = false;
        m_Agent.isStopped = true;

        m_Animator.speed = 0.5f;
        m_Animator.SetBool(m_bHashDamaged, true);
        m_Animator.SetBool(m_bHashUseSkill, false);

        for (int i = 0; i < 2; ++i)
        {
            if (!m_SkillObject[i])
                continue;

            Destroy(m_SkillObject[i]);
            m_SkillObject[i] = null;
        }

        int damagePercent = 0;
        ObjectManager.ABILITY eAbility = m_Target.GetComponent<Player>().GetAbility();
        switch (m_eAbility)
        {
            case ObjectManager.ABILITY.ABIL_FIRE:
                {
                    if (eAbility == ObjectManager.ABILITY.ABIL_FIRE)
                        damagePercent = 2;
                    else if (eAbility == ObjectManager.ABILITY.ABIL_GRASS)
                        damagePercent = 1;
                    else if (eAbility == ObjectManager.ABILITY.ABIL_WATER)
                        damagePercent = 3;
                }
                break;
            case ObjectManager.ABILITY.ABIL_GRASS:
                {
                    if (eAbility == ObjectManager.ABILITY.ABIL_FIRE)
                        damagePercent = 3;
                    else if (eAbility == ObjectManager.ABILITY.ABIL_GRASS)
                        damagePercent = 2;
                    else if (eAbility == ObjectManager.ABILITY.ABIL_WATER)
                        damagePercent = 1;
                }
                break;
            case ObjectManager.ABILITY.ABIL_WATER:
                {
                    if (eAbility == ObjectManager.ABILITY.ABIL_FIRE)
                        damagePercent = 1;
                    else if (eAbility == ObjectManager.ABILITY.ABIL_GRASS)
                        damagePercent = 3;
                    else if (eAbility == ObjectManager.ABILITY.ABIL_WATER)
                        damagePercent = 2;
                }
                break;
            default:
                damagePercent = 10;
                break;
        }

        m_HP -= damage * damagePercent;
        m_bDamaged = true;

        if (m_HP <= 0)
        {
            bool bDropJewelry = (Random.Range(0, 2) == 1)? true : false;

            PlayerProperty playerProperty = m_Target.GetComponent<PlayerProperty>();
            playerProperty.AddExperience(10 * playerProperty.GetPlayerStat().m_Level);

            m_Animator.SetBool("IsDead", true);

            if (bDropJewelry)
            {
                PlayerProperty.OBJTYPE eType = (PlayerProperty.OBJTYPE)Random.Range(2, 4);
                int amount = Random.Range(1, playerProperty.GetPlayerStat().m_Level + 1);

                playerProperty.AddProperty(eType, amount);
                GameObject.Find("UIManager").GetComponent<UIManager>().SetNoticeUI(eType, amount);
            }

            SetDestroy();
        }
    }

    public void ResetNormal()
    {
        m_bAttack = false;
        m_Animator.speed = 1f;

        if(m_eAbility == ObjectManager.ABILITY.ABIL_WATER)
            transform.Find("Body").GetComponent<BoxCollider>().isTrigger = false;

        m_Animator.SetBool(m_bHashDamaged, false);
        m_Animator.SetBool(m_bHashUseSkill, false);
    }

    public void SetDestroy()
    {
        // 이펙트 생성
        Instantiate(Resources.Load<GameObject>("Particle/Player/Grass/FireExplosion"), transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
