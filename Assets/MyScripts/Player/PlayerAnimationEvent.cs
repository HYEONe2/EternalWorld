using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    private Player m_Player;
    private Animator m_PlayerAnimator;

    // Start is called before the first frame update
    void Start()
    {
        if (!m_Player)
            m_Player = GameObject.Find("Player").GetComponent<Player>();
        if (!m_PlayerAnimator)
            m_PlayerAnimator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void StopSwingAxe()
    {
        if (m_PlayerAnimator)
        {
            m_Player.m_bSwingAxe = false;
            m_PlayerAnimator.SetBool("UseLButton", false);
        }
    }
}
