using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    private NavMeshAgent m_Agent;
    private GameObject m_Target;

    // Start is called before the first frame update
    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Target = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        m_Agent.destination = m_Target.transform.position;
    }
}
