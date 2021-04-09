using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMonster : MonoBehaviour
{
    private Animator m_Animator;

    private Player m_Player;

    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();

        m_Player = GameObject.Find("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_Animator.GetBool("m_bDamaged"))
            return;

        if (other.CompareTag("Weapon"))
        {
            //if (m_Player.GetAttack())
            {
                // 파티클 추가! 스파크 튀는 파티클!!
                m_Animator.SetBool("m_bDamaged", true);
            }
        }
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if (m_Animator.GetBool("m_bDamaged"))
    //        return;

    //    if (other.CompareTag("Weapon"))
    //    {
    //        //if (m_Player.GetAttack())
    //        {
    //            // 파티클 추가! 스파크 튀는 파티클!!
    //            m_Animator.SetBool("m_bDamaged", true);
    //        }
    //    }
    //}

    public void SetDamagedEnd()
    {
        m_Animator.SetBool("m_bDamaged", false);
    }
}
