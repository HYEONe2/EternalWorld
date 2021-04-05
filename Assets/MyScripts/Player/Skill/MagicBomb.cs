using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBomb : MonoBehaviour
{
    private ParticleSystem m_PartiSystem;
    private SphereCollider m_Collider;

    private bool m_bStop;
    private Vector3 m_LookVector;
    private float m_Speed;

    private float m_StopTime;

    public void SetLookVector(Vector3 lookVec) { m_LookVector = lookVec; }

    // Start is called before the first frame update
    void Start()
    {
        m_PartiSystem = GetComponent<ParticleSystem>();
        m_Collider = GetComponent<SphereCollider>();

        m_bStop = false;
        m_Speed = 10f;
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
            if (m_StopTime > 5f)
            {
                GameObject.Find("Player").GetComponent<Player>().SetSkillObject(1, null);
                Destroy(gameObject);
                return;
            }
            else
                m_StopTime += Time.deltaTime;

            if (m_PartiSystem.time >= 1f)
                m_PartiSystem.time = 0f;

            transform.position += m_LookVector * Time.deltaTime * m_Speed;
        }
        else
        {
            if (m_PartiSystem.time > 1.5f)
                m_Collider.radius = 2.5f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            m_bStop = true;
            GameObject.Find("Player").GetComponent<Player>().SetSkillObject(1, null);
        }
    }
}
