using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpParticle : MonoBehaviour
{
    private Transform m_PlayerTrans;

    private float m_StopTime;

    // Start is called before the first frame update
    void Start()
    {
        m_PlayerTrans = GameObject.FindWithTag("Player").transform;

        ParticleSystem.MainModule partiSystem = GetComponent<ParticleSystem>().main;
        partiSystem.simulationSpeed = 3f;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_StopTime > 2f)
        {
            Destroy(gameObject);
            return;
        }
        else
            m_StopTime += Time.deltaTime;

        transform.position = m_PlayerTrans.position;
    }
}
