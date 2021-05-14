using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RazorAttack : MonoBehaviour
{
    private float m_DestroyTime;

    // Start is called before the first frame update
    void Start()
    {
        m_DestroyTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_DestroyTime > 3f)
        {
            m_DestroyTime = 0;
            gameObject.SetActive(false);
            return;
        }

        m_DestroyTime += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player.GetAbility() == ObjectManager.ABILITY.ABIL_WATER && player.GetSkillObject(2))
                return;

            //if (!player.GetDamaged())
            //{
            //    PlayerProperty playerProperty = other.GetComponent<PlayerProperty>();
            //    playerProperty.SetDamaged(ObjectManager.ABILITY.ABIL_GRASS, playerProperty.GetPlayerStat().m_Level);

                m_DestroyTime = 2f;
            //}
        }
    }
}
