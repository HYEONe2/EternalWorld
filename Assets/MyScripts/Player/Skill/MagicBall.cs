using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBall : MonoBehaviour
{
    private Vector3 m_LookVector;
    private float m_Speed;

    private float m_DestroyTime;

    public void SetLookVector(Vector3 lookVec) { m_LookVector = lookVec; }

    // Start is called before the first frame update
    void Start()
    {
        m_Speed = 10f;

        m_DestroyTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_DestroyTime > 3f)
        {
            GameObject.Find("Player").GetComponent<Player>().SetSkillObject(0, null);
            Destroy(gameObject);
            return;
        }

        transform.position += m_LookVector * Time.deltaTime * m_Speed;
        m_DestroyTime += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Monster"))
        {
            GameObject.Find("Player").GetComponent<Player>().SetSkillObject(0, null);
            Destroy(gameObject);
        }
    }
}
