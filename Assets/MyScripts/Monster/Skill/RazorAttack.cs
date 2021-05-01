using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RazorAttack : MonoBehaviour
{
    private float m_DestroyTime;

    // Start is called before the first frame update
    void Start()
    {
        m_DestroyTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_DestroyTime > 3f)
        {
            m_DestroyTime = 0;
            gameObject.SetActive(false);
            return;
        }

        m_DestroyTime += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(!other.GetComponent<Player>().GetDamaged())
                other.GetComponent<PlayerProperty>().SetDamaged(1);
        }
    }
}
