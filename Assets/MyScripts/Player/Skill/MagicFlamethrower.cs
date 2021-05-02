using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicFlamethrower : MonoBehaviour
{
    private GameObject m_Master;
    private ParticleSystem m_PartiSystem;

    // Start is called before the first frame update
    void Start()
    {
        m_PartiSystem = GetComponent<ParticleSystem>();

        if (CompareTag("Weapon"))
            m_Master = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_PartiSystem.IsAlive())
        {
            if (CompareTag("Weapon"))
                m_Master.GetComponent<Player>().SetSkillObject(2, null);
            Destroy(transform.parent.gameObject);
            return;
        }

        if (CompareTag("Weapon"))
        {
            Vector3 PlayerPos = m_Master.transform.position;
            Vector3 PlayerLook = m_Master.transform.Find("PlayerMesh").gameObject.transform.forward;
            Vector3 NewPos = NewPos = PlayerPos + PlayerLook;
            NewPos.y += 1f;

            transform.parent.position = NewPos;
            transform.parent.rotation = Quaternion.LookRotation(PlayerLook);
        }
    }

    private void OnDestroy()
    {
        if (CompareTag("Weapon"))
            GameObject.Find("Player").GetComponent<Player>().SetSkillObject(2, null);
        Destroy(transform.parent.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (CompareTag("Weapon"))
        {
            if (other.CompareTag("Monster"))
            {
                Monster monster = other.GetComponent<Monster>();
                if (monster && !monster.GetDamaged())
                    monster.SetDamaged(1);
            }
        }
        else if (CompareTag("MonsterWeapon"))
        {
            if (other.CompareTag("Player"))
            {
                PlayerProperty playerProperty = other.GetComponent<PlayerProperty>();
                playerProperty.SetDamaged(ObjectManager.ABILITY.ABIL_FIRE, playerProperty.GetLevel());
            }
        }
    }
}
