using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicShield : MonoBehaviour
{
    private float m_DestroyTime;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 Pos;

        if (m_DestroyTime > 3f)
        {
            Pos = transform.position;
            Pos.y -= 1f;

            Instantiate(Resources.Load<GameObject>("Particle/Player/Water/MagicShieldWave"), Pos, Quaternion.identity);
            Destroy(gameObject);
            return;
        }

        Pos = GameObject.Find("Player").transform.position;
        Pos.y += 0.5f;
        transform.position = Pos;

        m_DestroyTime += Time.deltaTime;
    }
}
