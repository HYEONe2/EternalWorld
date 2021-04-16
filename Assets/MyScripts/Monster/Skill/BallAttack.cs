using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallAttack : MonoBehaviour
{
    private Vector3 m_LookVector;
    private float m_Speed;

    private float m_DestroyTime;

    public void SetLookVector(Vector3 lookVec) { m_LookVector = lookVec; }

    // Start is called before the first frame update
    void Start()
    {
        m_Speed = 5f;

        m_DestroyTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_DestroyTime > 5f)
        {
            Destroy(gameObject);
            return;
        }

        transform.position += m_LookVector * Time.deltaTime * m_Speed;
        m_DestroyTime += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerProperty>().SetDamaged(1);
            Destroy(gameObject);
        }
    }
}
