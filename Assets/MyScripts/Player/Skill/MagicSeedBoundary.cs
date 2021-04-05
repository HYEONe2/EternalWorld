using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicSeedBoundary : MonoBehaviour
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
            GameObject.Find("Player").GetComponent<Player>().SetSkillObject(2, null);
            Destroy(gameObject);
            return;
        }

        m_DestroyTime += Time.deltaTime;
    }

    public void RespawnSeeds(int amount)
    {
        Vector3 BoundaryLoc = transform.position;
        Vector3 BoundaryScale = GetComponent<BoxCollider>().size;

        for (int i = 0; i < amount; ++i)
        {
            Vector3 RandomLoc = new Vector3(Random.Range(BoundaryLoc.x - BoundaryScale.x * 0.5f, BoundaryLoc.x + BoundaryScale.x * 0.5f),
                Random.Range(BoundaryLoc.y + BoundaryScale.y * 0.5f, BoundaryLoc.y - BoundaryScale.y * 0.5f), 
                Random.Range(BoundaryLoc.z - BoundaryScale.z * 0.5f, BoundaryLoc.z + BoundaryScale.z * 0.5f));
            Instantiate(Resources.Load<GameObject>("Particle/Player/Grass/Seed"), RandomLoc, Quaternion.identity);
        }
    }
}
