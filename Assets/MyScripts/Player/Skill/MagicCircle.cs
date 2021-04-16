using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicCircle : MonoBehaviour
{
    private ParticleSystem m_PartiSystem;

    // Start is called before the first frame update
    void Start()
    {
        m_PartiSystem = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_PartiSystem.IsAlive())
        {
            GameObject.Find("Player").GetComponent<Player>().SetSkillObject(1, null);
            Destroy(transform.parent.gameObject);
        }
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
