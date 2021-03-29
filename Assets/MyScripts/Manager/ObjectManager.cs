using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    private Transform m_PlayerTrans;

    private Transform m_TreeBoundaryTrans;
    private GameObject[] m_Trees;
    private GameObject[] m_Rocks;

    private Transform m_LevelBoundaryTrans;
    List<Transform> m_LevelChildTrans = new List<Transform>();
    List<Material> m_LevelChildMat = new List<Material>();

    private bool m_bCloseEnough;

    public bool GetCloseEnough() { return m_bCloseEnough; }

    // Start is called before the first frame update
    void Start()
    {
        m_PlayerTrans = GameObject.Find("Player").transform;
        m_TreeBoundaryTrans = transform.Find("TreeBoundary").transform;
        m_LevelBoundaryTrans = transform.Find("LevelBoundary").transform;
        for (int i = 0; i < m_LevelBoundaryTrans.childCount; ++i)
        {
            m_LevelChildTrans.Add(m_LevelBoundaryTrans.GetChild(i));
            m_LevelChildMat.Add(m_LevelBoundaryTrans.GetChild(i).GetComponent<MeshRenderer>().material);
        }

        m_Trees = Resources.LoadAll<GameObject>("Object/Commodity/Tree");
        m_Rocks = Resources.LoadAll<GameObject>("Object/Commodity/Rock");
    }

    // Update is called once per frame
    void Update()
    {
        UpdateBoundary();
    }

    private void UpdateBoundary()
    {
        if (Input.GetKeyDown("4"))
        {
            RespawnTrees(3);
            RespawnRocks(3);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            m_LevelChildTrans.Clear();
            m_LevelChildMat.Clear();
            Destroy(m_LevelBoundaryTrans.gameObject);
        }

        if (CheckBoundaryDistance())
            m_bCloseEnough = true;
        else
            m_bCloseEnough = false;
    }

    public void RespawnTrees(int amount)
    {
        Vector3 TreeBoundaryLoc = m_TreeBoundaryTrans.position;
        Vector3 TreeBoundaryScale = m_TreeBoundaryTrans.localScale;

        for (int i = 0; i < amount; ++i)
        {
            //Vector3 RandomLoc = new Vector3(Random.Range(TreeBoundaryLoc.x - TreeBoundaryScale.x * 0.5f, TreeBoundaryLoc.x + TreeBoundaryScale.x * 0.5f), (TreeBoundaryLoc.y - TreeBoundaryScale.y * 0.5f), Random.Range(TreeBoundaryLoc.z - TreeBoundaryScale.z * 0.5f, TreeBoundaryLoc.z + TreeBoundaryScale.z * 0.5f));
            Vector3 RandomLoc = new Vector3(Random.Range(TreeBoundaryLoc.x - TreeBoundaryScale.x * 0.5f, TreeBoundaryLoc.x + TreeBoundaryScale.x * 0.5f), (TreeBoundaryLoc.y + TreeBoundaryScale.y * 0.5f), Random.Range(TreeBoundaryLoc.z - TreeBoundaryScale.z * 0.5f, TreeBoundaryLoc.z + TreeBoundaryScale.z * 0.5f));

            GameObject Tree = Instantiate(m_Trees[Random.Range(0, m_Trees.Length)], RandomLoc, new Quaternion(0, 0, 0, 0));
        }
    }

    public void RespawnRocks(int amount)
    {
        Vector3 TreeBoundaryLoc = m_TreeBoundaryTrans.position;
        Vector3 TreeBoundaryScale = m_TreeBoundaryTrans.localScale;

        for (int i = 0; i < amount; ++i)
        {
            //Vector3 RandomLoc = new Vector3(Random.Range(TreeBoundaryLoc.x - TreeBoundaryScale.x * 0.5f, TreeBoundaryLoc.x + TreeBoundaryScale.x * 0.5f), (TreeBoundaryLoc.y - TreeBoundaryScale.y * 0.5f), Random.Range(TreeBoundaryLoc.z - TreeBoundaryScale.z * 0.5f, TreeBoundaryLoc.z + TreeBoundaryScale.z * 0.5f));
            Vector3 RandomLoc = new Vector3(Random.Range(TreeBoundaryLoc.x - TreeBoundaryScale.x * 0.5f, TreeBoundaryLoc.x + TreeBoundaryScale.x * 0.5f), (TreeBoundaryLoc.y + TreeBoundaryScale.y * 0.5f), Random.Range(TreeBoundaryLoc.z - TreeBoundaryScale.z * 0.5f, TreeBoundaryLoc.z + TreeBoundaryScale.z * 0.5f));

            GameObject Rock = Instantiate(m_Rocks[Random.Range(0, m_Rocks.Length)], RandomLoc, new Quaternion(0, 0, 0, 0));
            Rock.transform.localScale = new Vector3(Random.Range(1.5f, 2f), Random.Range(1.5f, 2f), Random.Range(1.5f, 2f));
        }
    }

    private bool CheckBoundaryDistance()
    {
        Vector3 PlayerPos = m_PlayerTrans.position;

        for(int i = 0; i < m_LevelChildTrans.Count; ++i)
        {
            Vector3 ChildTrans = m_LevelChildTrans[i].position;
            float Dist = (PlayerPos - ChildTrans).magnitude;

            //if(i == 0)
            //    Debug.Log(Dist);

            if (Dist < 4f)
            {
                SetTransparent(m_LevelChildMat[i], 1f - (Dist - 3f));
                return true;
            }
            else
            {
                SetTransparent(m_LevelChildMat[i], 0f);
            }
        }

        return false;
    }

    private void SetTransparent(Material material, float alpha)
    {
        material.SetFloat("_Mode", 3f);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;

        material.color = new Color(0f, 0f, 0f, alpha);
    }

    public void DestroyLevelBoundary()
    {
        m_LevelChildTrans.Clear();
        m_LevelChildMat.Clear();
        Destroy(m_LevelBoundaryTrans.gameObject);
    }
}
