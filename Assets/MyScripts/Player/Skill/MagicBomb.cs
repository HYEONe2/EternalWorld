using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBomb : MonoBehaviour
{
    private ParticleSystem m_PartiSystem;
    private SphereCollider m_Collider;
    private Monster m_Monster;

    private bool m_bStop;
    private float m_StopTime;

    private Vector3 m_LookVector;
    private float m_Speed;

    private bool m_bAttacked;

    // Start is called before the first frame update
    void Start()
    {
        m_PartiSystem = GetComponent<ParticleSystem>();
        m_Collider = GetComponent<SphereCollider>();

        m_bStop = false;
        m_StopTime = 0;

        m_LookVector = GameObject.Find("Player").transform.Find("PlayerMesh").forward;
        m_Speed = 5f;

        m_bAttacked = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_PartiSystem.IsAlive())
        {
            GameObject.Find("Player").GetComponent<Player>().SetSkillObject(1, null);
            Destroy(gameObject);
            return;
        }

        if (!m_bStop)
        {
            m_PartiSystem.time = 0f;

            if (m_StopTime > 5f)
            {
                GameObject.Find("Player").GetComponent<Player>().SetSkillObject(1, null);
                Destroy(gameObject);
                return;
            }
            else
                m_StopTime += Time.deltaTime;

            transform.position += m_LookVector * Time.deltaTime * m_Speed;
        }
        else
        {
            if(m_PartiSystem.time > 0.5f)
            {
                if (!GetComponent<AudioSource>().isPlaying)
                    GetComponent<AudioSource>().Play();
            }
            else if (m_PartiSystem.time > 1.5f)
            {
                m_Collider.radius = 2.5f;

                if (m_bAttacked || !m_Monster)
                    return;

                m_Monster.SetDamaged(1);
                m_bAttacked = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_Monster)
            return;

        if (other.CompareTag("Monster"))
        {
            m_Monster = other.GetComponent<Monster>();
            m_bStop = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Monster"))
            m_Monster = null;
    }
}
