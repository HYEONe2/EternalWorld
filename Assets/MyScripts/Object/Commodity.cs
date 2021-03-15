using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Commodity : MonoBehaviour
{
    public enum OBJTYPE
    {
        OBJ_TREE,
        OBJ_ROCK,
        OBJ_END
    };

    public OBJTYPE m_eType;

    private Player m_Player;
    private GameObject m_PlayerEquip;
    private Animator m_PlayerAnimator;
    private GameObject m_HurtParticle;
    private GameObject m_Particle;
    private GameObject m_Reward;

    private bool m_bLateInit;
    private bool m_bCheckAttack;
    private bool m_bCheckHurt;
    private int m_HP;

    // Function
    public void SetCheckAttack(bool bCheck) { m_bCheckAttack = bCheck; }

    // Start is called before the first frame update
    void Start()
    {
        if (!m_Player) m_Player = GameObject.Find("Player").GetComponent<Player>();
        if (!m_PlayerEquip) m_PlayerEquip = GameObject.FindWithTag("Equipment");
        if (!m_PlayerAnimator) m_PlayerAnimator = GameObject.Find("PlayerMesh").GetComponent<Animator>();
        if (!m_Particle) m_Particle = Resources.Load<GameObject>("Particle/Commodity/WFX_Explosion Simple");

        m_bLateInit = false;
        m_bCheckAttack = false;
        m_bCheckHurt = false;
        m_HP = 5;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(m_HP);

        if (!m_bLateInit)
            InitializeParticle();

        if (!m_PlayerAnimator.GetBool("UseLButton"))
            m_bCheckAttack = false;

        if (!m_bCheckAttack && m_bCheckHurt)
        {
            m_HP -= 1;
            m_bCheckHurt = false;
        }

        if (m_HP <= 0)
        {
            Instantiate(m_Particle, transform.position, new Quaternion(0, 0, 0, 0));
            GameObject Reward = Instantiate(m_Reward, transform.position, new Quaternion(-90f, 0, 0, 0));

            if(m_eType == OBJTYPE.OBJ_TREE)
                Reward.GetComponent<Reward>().SetObjType(global::Reward.OBJTYPE.OBJ_TREE);
            else if(m_eType == OBJTYPE.OBJ_ROCK)
                Reward.GetComponent<Reward>().SetObjType(global::Reward.OBJTYPE.OBJ_ROCK);

            m_Player.SetNearObject(null);
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Equipment"))
        {
            m_Player.SetNearObject(this.gameObject);

            //if (m_PlayerAnimator.GetBool("UseLButton"))
            //    m_bCheckAttack = true;

            if (m_bCheckAttack && !m_bCheckHurt)
            {
                Instantiate(m_HurtParticle, other.transform.position, new Quaternion(0, 0, 0, 0));
                m_bCheckHurt = true;
            }
        }
    }

    void InitializeParticle()
    {
        if (m_eType == OBJTYPE.OBJ_END)
            return;

        switch(m_eType)
        {
            case OBJTYPE.OBJ_TREE:
                m_HurtParticle = Resources.Load<GameObject>("Particle/Commodity/WFX_BImpact Wood");
                m_Reward = Resources.Load<GameObject>("Object/Reward/Firewood/12303_Firewood_Stack_v1_l3");
                //m_HurtParticle.transform.localScale = new Vector3(2f, 2f, 2f);
                break;
            case OBJTYPE.OBJ_ROCK:
                m_HurtParticle = Resources.Load<GameObject>("Particle/Commodity/WFX_BImpact Concrete");
                int rand = Random.Range(0, 2);
                if (rand == 0)
                    m_Reward = Resources.Load<GameObject>("Object/Reward/Rock/P_Rock_01");
                else if (rand == 1)
                    m_Reward = Resources.Load<GameObject>("Object/Reward/Rock/P_Rock_02");
                else
                    m_Reward = Resources.Load<GameObject>("Object/Reward/Rock/P_Rock_03");
                //m_HurtParticle.transform.localScale = new Vector3(2f, 2f, 2f);
                break;
        }

        m_bLateInit = true;
    }
}
