using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    private Player m_Player;
    private PlayerProperty m_PlayerProperty;
    private Animator m_Animator;

    // Animation Values
    private readonly int m_bHashDamaged = Animator.StringToHash("IsDamaged");
    private readonly int m_bHashDead = Animator.StringToHash("IsDead");

    // Start is called before the first frame update
    void Start()
    {
        m_Player = transform.parent.GetComponent<Player>();
        m_PlayerProperty = transform.parent.GetComponent<PlayerProperty>();
        m_Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerStay(Collider other)
    {
        if (m_Player.GetTarget())
            return;

        if (other.CompareTag("Monster"))
            m_Player.SetTargetMonster(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Monster"))
            m_Player.SetTargetMonster(null);
    }

    void StopSwingAxe()
    {
        if (m_Animator)
        {
            m_Player.SetSwing(false);
            m_Animator.SetBool("UseLButton", false);
        }
    }

    void StartAttack()
    {
        if (!m_Player.GetNearObject())
            return;

        Commodity commodity = m_Player.GetNearObject().GetComponent<Commodity>();

        if (commodity)
            commodity.SetCheckAttack(true);
    }

    public void ResetNormal()
    {
        m_Animator.SetBool(m_bHashDamaged, false);

        int HP = m_PlayerProperty.GetHP();
        if (HP <= 0)
            m_Animator.SetBool(m_bHashDead, true);
    }
}
