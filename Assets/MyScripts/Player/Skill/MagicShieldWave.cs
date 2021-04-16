using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicShieldWave : MonoBehaviour
{
    private ParticleSystem m_PartiSystem;
    private SphereCollider m_Collider;

    // Start is called before the first frame update
    void Start()
    {
        m_PartiSystem = GetComponent<ParticleSystem>();
        m_Collider = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_PartiSystem.IsAlive())
        {
            GameObject.Find("Player").GetComponent<Player>().SetSkillObject(2, null);
            Destroy(gameObject);
        }

        if (m_Collider.radius <= 0.5f)
            m_Collider.radius += m_PartiSystem.time * 0.4f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            Monster monster = other.GetComponent<Monster>();
            if (monster)
                monster.SetDamaged(1);
        }
    }
}
