using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    private Transform m_TreeBoundaryTrans;
    private GameObject[] m_Trees;
    private GameObject[] m_Rocks;
    //static List<GameObject> m_Tree = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        if (!m_TreeBoundaryTrans) m_TreeBoundaryTrans = GameObject.Find("TreeBoundary").transform;

        m_Trees = Resources.LoadAll<GameObject>("Object/Commodity/Tree");
        m_Rocks = Resources.LoadAll<GameObject>("Object/Commodity/Rock");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("4"))
        {
            RespawnTrees(3);
            RespawnRocks(3);
        }
    }

    void RespawnTrees(int amount)
    {
        Vector3 TreeBoundaryLoc = m_TreeBoundaryTrans.position;
        Vector3 TreeBoundaryScale = m_TreeBoundaryTrans.localScale;

        for (int i = 0; i < amount; ++i)
        {
            Vector3 RandomLoc = new Vector3(Random.Range(TreeBoundaryLoc.x - TreeBoundaryScale.x * 0.5f, TreeBoundaryLoc.x + TreeBoundaryScale.x * 0.5f), (TreeBoundaryLoc.y - TreeBoundaryScale.y * 0.5f), Random.Range(TreeBoundaryLoc.z - TreeBoundaryScale.z * 0.5f, TreeBoundaryLoc.z + TreeBoundaryScale.z * 0.5f));

            GameObject Tree = Instantiate(m_Trees[Random.Range(0, m_Trees.Length)], RandomLoc, new Quaternion(0, 0, 0, 0));
        }
    }

    void RespawnRocks(int amount)
    {
        Vector3 TreeBoundaryLoc = m_TreeBoundaryTrans.position;
        Vector3 TreeBoundaryScale = m_TreeBoundaryTrans.localScale;

        for (int i = 0; i < amount; ++i)
        {
            Vector3 RandomLoc = new Vector3(Random.Range(TreeBoundaryLoc.x - TreeBoundaryScale.x * 0.5f, TreeBoundaryLoc.x + TreeBoundaryScale.x * 0.5f), (TreeBoundaryLoc.y - TreeBoundaryScale.y * 0.5f), Random.Range(TreeBoundaryLoc.z - TreeBoundaryScale.z * 0.5f, TreeBoundaryLoc.z + TreeBoundaryScale.z * 0.5f));

            GameObject Rock = Instantiate(m_Rocks[Random.Range(0, m_Rocks.Length)], RandomLoc, new Quaternion(0, 0, 0, 0));
            Rock.transform.localScale = new Vector3(Random.Range(1.5f, 2f), Random.Range(1.5f, 2f), Random.Range(1.5f, 2f));
        }
    }
}
