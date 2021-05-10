using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonAttack : MonoBehaviour
{
    private ParticleSystem m_PartiSystem;
    private Transform m_PlayerTrans;

    // Start is called before the first frame update
    void Start()
    {
        m_PartiSystem = GetComponent<ParticleSystem>();
        m_PlayerTrans = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_PartiSystem.IsAlive())
        {
            Destroy(transform.parent.gameObject);
            return;
        }

        Vector3 target = m_PlayerTrans.position - transform.position;
        target.Normalize();

        transform.position += target * Time.deltaTime * 5f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player.GetAbility() == ObjectManager.ABILITY.ABIL_WATER && player.GetSkillObject(2))
                return;

            PlayerProperty playerProperty = other.GetComponent<PlayerProperty>();
            playerProperty.SetDamaged(ObjectManager.ABILITY.ABIL_GRASS, playerProperty.GetPlayerStat().m_Level);
        }
    }
}
