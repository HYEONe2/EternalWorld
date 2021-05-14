using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Commodity : MonoBehaviour
{
    public PlayerProperty.OBJTYPE m_eType;

    private Player m_Player;
    private PlayerProperty m_PlayerProperty;
    private GameObject m_PlayerEquip;
    private Animator m_PlayerAnimator;
    private AudioSource m_EffectSound;

    private GameObject m_HurtParticle;
    private GameObject m_Particle;
    private GameObject m_Reward;

    private bool m_bLateInit;
    private bool m_bCheckAttack;
    private bool m_bCheckHurt;

    private int m_HP;
    private int m_Exp;

    // Function
    public void SetCheckAttack(bool bCheck) { m_bCheckAttack = bCheck; }

    // Start is called before the first frame update
    void Start()
    {
        GameObject Player = GameObject.Find("Player");

        m_Player = Player.GetComponent<Player>();
        m_PlayerProperty = Player.GetComponent<PlayerProperty>();
        m_PlayerEquip = GameObject.FindWithTag("Equipment");
        m_PlayerAnimator = Player.transform.Find("PlayerMesh").GetComponent<Animator>();
        m_Particle = Resources.Load<GameObject>("Particle/Commodity/WFX_Explosion Simple");
        m_EffectSound = GetComponent<AudioSource>();

        m_bLateInit = false;
        m_bCheckAttack = false;
        m_bCheckHurt = false;

        m_HP = 5;
        m_Exp = 10;
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
            GameObject Reward = Instantiate(m_Reward, transform.position, new Quaternion(0, 0, 0, 0));

            if(m_eType == PlayerProperty.OBJTYPE.OBJ_WOOD)
                Reward.GetComponent<Reward>().SetObjType(PlayerProperty.OBJTYPE.OBJ_WOOD);
            else if(m_eType == PlayerProperty.OBJTYPE.OBJ_STONE)
                Reward.GetComponent<Reward>().SetObjType(PlayerProperty.OBJTYPE.OBJ_STONE);

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
                m_EffectSound.Play();

                m_PlayerProperty.AddExperience(m_Exp);
                m_bCheckHurt = true;
            }
        }
    }

    void InitializeParticle()
    {
        if (m_eType == PlayerProperty.OBJTYPE.OBJ_END)
            return;

        switch(m_eType)
        {
            case PlayerProperty.OBJTYPE.OBJ_WOOD:
                m_HurtParticle = Resources.Load<GameObject>("Particle/Commodity/WFX_BImpact Wood");
                m_Reward = Resources.Load<GameObject>("Object/Reward/Firewood/Firewood");
                //m_HurtParticle.transform.localScale = new Vector3(2f, 2f, 2f);
                break;
            case PlayerProperty.OBJTYPE.OBJ_STONE:
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
