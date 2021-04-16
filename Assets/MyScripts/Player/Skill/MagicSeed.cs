using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicSeed : MonoBehaviour
{
    private float m_DestroyTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_DestroyTime > 3f)
        {
            Destroy(gameObject);
            return;
        }

        m_DestroyTime += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster")
            || other.CompareTag("Player"))
        {
            Monster monster = other.GetComponent<Monster>();
            if (monster)
                monster.SetDamaged(1);

            //Instantiate(Resources.Load<GameObject>("Particle/Player/Grass/FireExplosion"), transform.position, Quaternion.identity);
            Destroy(gameObject);
            // 몬스터 걸음 멈춤
        }
    }
}
