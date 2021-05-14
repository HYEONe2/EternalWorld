using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicWall : MonoBehaviour
{
    private ParticleSystem m_PartiSystem;

    // Start is called before the first frame update
    void Start()
    {
        m_PartiSystem = transform.GetChild(0).GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_PartiSystem.IsAlive())
        {
            GameObject.Find("Player").GetComponent<Player>().SetSkillObject(1, null);
            DestroyWall();
        }
    }

    private void DestroyWall()
    {
        for (int i = 0; i < transform.childCount; ++i)
            Destroy(transform.GetChild(i).gameObject);
        Destroy(gameObject);
    }
}
