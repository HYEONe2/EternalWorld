using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBoundary : MonoBehaviour
{
    [SerializeField] public int m_Level;
    private Transform m_Trans;

    private Transform m_EnvironmentTrans;
    private GameObject[] m_Trees;
    private GameObject[] m_Rocks;

    private PlayerProperty m_PlayerProperty;

    // Start is called before the first frame update
    void Start()
    {
        m_Trans = transform;

        m_EnvironmentTrans = GameObject.Find("Environment").transform;
        m_Trees = Resources.LoadAll<GameObject>("Object/Commodity/Tree");
        m_Rocks = Resources.LoadAll<GameObject>("Object/Commodity/Rock");

        m_PlayerProperty = GameObject.Find("Player").GetComponent<PlayerProperty>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLevel();
    }

    private void UpdateLevel()
    {
        if (m_PlayerProperty.GetPlayerStat().m_Level != m_Level)
            return;

        RespawnObjects(5, 5);
    }

    public void RespawnObjects(int treeAmount, int rockAmount)
    {
        if (treeAmount == 0 && rockAmount == 0)
            return;

        if(treeAmount != 0)
            RespawnTrees(treeAmount);
        
        if(rockAmount != 0)
            RespawnRocks(treeAmount);

        Destroy(gameObject);
    }

    private void RespawnTrees(int amount)
    {
        Vector3 TreeBoundaryLoc = m_Trans.position;
        Vector3 TreeBoundaryScale = m_Trans.localScale;

        for (int i = 0; i < amount; ++i)
        {
            //Vector3 RandomLoc = new Vector3(Random.Range(TreeBoundaryLoc.x - TreeBoundaryScale.x * 0.5f, TreeBoundaryLoc.x + TreeBoundaryScale.x * 0.5f), (TreeBoundaryLoc.y - TreeBoundaryScale.y * 0.5f), Random.Range(TreeBoundaryLoc.z - TreeBoundaryScale.z * 0.5f, TreeBoundaryLoc.z + TreeBoundaryScale.z * 0.5f));
            Vector3 RandomLoc = new Vector3(Random.Range(TreeBoundaryLoc.x - TreeBoundaryScale.x * 0.5f, TreeBoundaryLoc.x + TreeBoundaryScale.x * 0.5f), (TreeBoundaryLoc.y + TreeBoundaryScale.y * 0.5f), Random.Range(TreeBoundaryLoc.z - TreeBoundaryScale.z * 0.5f, TreeBoundaryLoc.z + TreeBoundaryScale.z * 0.5f));

            GameObject Tree = Instantiate(m_Trees[Random.Range(0, m_Trees.Length)], RandomLoc, new Quaternion(0, 0, 0, 0));
            Tree.transform.parent = m_EnvironmentTrans;
        }
    }

    private void RespawnRocks(int amount)
    {
        Vector3 TreeBoundaryLoc = m_Trans.position;
        Vector3 TreeBoundaryScale = m_Trans.localScale;

        for (int i = 0; i < amount; ++i)
        {
            //Vector3 RandomLoc = new Vector3(Random.Range(TreeBoundaryLoc.x - TreeBoundaryScale.x * 0.5f, TreeBoundaryLoc.x + TreeBoundaryScale.x * 0.5f), (TreeBoundaryLoc.y - TreeBoundaryScale.y * 0.5f), Random.Range(TreeBoundaryLoc.z - TreeBoundaryScale.z * 0.5f, TreeBoundaryLoc.z + TreeBoundaryScale.z * 0.5f));
            Vector3 RandomLoc = new Vector3(Random.Range(TreeBoundaryLoc.x - TreeBoundaryScale.x * 0.5f, TreeBoundaryLoc.x + TreeBoundaryScale.x * 0.5f), (TreeBoundaryLoc.y + TreeBoundaryScale.y * 0.5f), Random.Range(TreeBoundaryLoc.z - TreeBoundaryScale.z * 0.5f, TreeBoundaryLoc.z + TreeBoundaryScale.z * 0.5f));

            GameObject Rock = Instantiate(m_Rocks[Random.Range(0, m_Rocks.Length)], RandomLoc, new Quaternion(0, 0, 0, 0));
            Rock.transform.localScale = new Vector3(Random.Range(1.5f, 2f), Random.Range(1.5f, 2f), Random.Range(1.5f, 2f));
            Rock.transform.parent = m_EnvironmentTrans;
        }
    }
}
