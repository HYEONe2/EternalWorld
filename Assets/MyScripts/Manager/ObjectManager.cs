using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectManager : MonoBehaviour
{
    public enum ABILITY { ABIL_FIRE, ABIL_WATER, ABIL_GRASS, ABIL_END };

    private string m_SceneName;
    private Transform m_PlayerTrans;
    private PlayerProperty m_PlayerProperty;
    private int m_Level;

    private GameObject m_BuildingManager;
    private GameObject m_Boundary;
    private GameObject m_Environment;

    private List<LevelBoundary> m_LevelBoundaryList = new List<LevelBoundary>();
    private List<TreeBoundary> m_TreeBoundaryList = new List<TreeBoundary>();
    private List<Building> m_LandmarkList = new List<Building>();

    private static Color m_PingpongColor;

    public string GetSceneName() { return m_SceneName; }
    public void SetLandmark(Building eInfo) { m_LandmarkList.Add(eInfo); }

    // Start is called before the first frame update
    void Start()
    {
        m_SceneName = SceneManager.GetActiveScene().name;
        m_PlayerTrans = GameObject.Find("Player").transform;
        m_PlayerProperty = m_PlayerTrans.gameObject.GetComponent<PlayerProperty>();
        m_Level = 0;

        m_BuildingManager = transform.Find("BuildingManager").gameObject;
        m_Boundary = transform.Find("Boundary").gameObject;
        m_Environment = transform.Find("Environment").gameObject;

        m_Boundary.SetActive(false);
        m_Environment.SetActive(false);

        InitializeBoundary();
    }

    // Update is called once per frame
    void Update()
    {
        if (UpdateScene())
            return;

        CheatKey();
        UpdateBoundary();
        UpdateLandmark();
    }

    private void InitializeBoundary()
    {
        m_LevelBoundaryList.Clear();
        m_TreeBoundaryList.Clear();

        GameObject[] TempBoundary = GameObject.FindGameObjectsWithTag("LevelBoundary");
        foreach (GameObject iter in TempBoundary)
            m_LevelBoundaryList.Add(iter.GetComponent<LevelBoundary>());

        TempBoundary = GameObject.FindGameObjectsWithTag("TreeBoundary");
        foreach (GameObject iter in TempBoundary)
            m_TreeBoundaryList.Add(iter.GetComponent<TreeBoundary>());
    }

    private bool UpdateScene()
    {
        if (m_SceneName == SceneManager.GetActiveScene().name)
            return false;

        m_SceneName = SceneManager.GetActiveScene().name;
        if (m_SceneName == "MainScene")
        {
            m_BuildingManager.SetActive(true);
            m_Boundary.SetActive(true);
            m_Environment.SetActive(true);
        }
        else
        {
            m_Boundary.SetActive(false);
            m_Environment.SetActive(false);
            return false;
        }

        //foreach (LevelBoundary boundary in m_LevelBoundaryList)
        //{
        //    if (boundary)
        //        Destroy(boundary.gameObject);
        //}

        //foreach (TreeBoundary boundary in m_TreeBoundaryList)
        //{
        //    if (boundary)
        //        Destroy(boundary.gameObject);
        //}

        InitializeBoundary();

        return true;
    }

    private void CheatKey()
    {
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    TreeBoundary treeBoundary = GameObject.Find("TreeBoundary").GetComponent<TreeBoundary>();
        //    RespawnTrees(3);
        //    RespawnRocks(3);
        //}

        if (Input.GetKeyDown(KeyCode.L))
        {
            GameObject boundary = GameObject.Find("LevelBoundary");
            if (!boundary)
                return;

            LevelBoundary levelBoundary = boundary.GetComponent<LevelBoundary>();
            levelBoundary.DestroyLevelBoundary();
            return;
        }
    }

    private void UpdateBoundary()
    {
        int Level = m_PlayerProperty.GetPlayerStat().m_Level;
        if (m_Level == Level)
            return;

        foreach(TreeBoundary boundary in m_TreeBoundaryList)
        {
            if(boundary.m_Level == Level)
            {
                switch(boundary.m_Level)
                {
                    case 1:
                        boundary.RespawnObjects(5, 5);
                        break;
                    case 2:
                        boundary.RespawnObjects(4, 4);
                        break;
                }
            }
        }

        foreach (LevelBoundary boundary in m_LevelBoundaryList)
        {
            if(boundary.m_Level == Level)
            {
                Destroy(boundary.gameObject);
                m_LevelBoundaryList.Remove(boundary);
                break;
            }
        }
        m_Level = Level;
    }

    private void UpdateLandmark()
    {
        foreach(Building landmark in m_LandmarkList)
        {
            Building.BuildingInfo eInfo = landmark.GetBuildingInfo();

            if (landmark.GetCheckTime() >= eInfo.m_BuildTime)
            {
                PlayerProperty.PlayerStat stat = m_PlayerProperty.GetPlayerStat();
                switch (eInfo.m_eBuildingType)
                {
                    case Building.BUILDING.BUILDING_HEALHP:
                        // Player HP 올려주기
                        m_PlayerProperty.SetHP(stat.m_HP + 1);
                        break;
                    case Building.BUILDING.BUILDING_BUFFHP:
                        // PLayer HP 피통 올려주기
                        m_PlayerProperty.AddExtraHP(1);
                        break;
                    case Building.BUILDING.BUILDING_BUFFCOOL:
                        // Player Cooltime 올려주기
                        m_PlayerProperty.AddExtraCooltime(1);
                        break;
                    case Building.BUILDING.BUILDING_BUFFSTR:
                        // Player str 올려주기
                        m_PlayerProperty.AddStr(1);
                        break;
                }

                landmark.SetCheckTime(0); 
            }
            else
                landmark.SetCheckTime(landmark.GetCheckTime() + Time.deltaTime);
        }
    }

    public bool GetCloseEnough()
    {
        if (m_SceneName != "TutorialScene")
            return false;

        LevelBoundary levelBoundary = GameObject.Find("LevelBoundary").GetComponent<LevelBoundary>();
        if (levelBoundary.GetCloseEnough())
            return true;
        else
            return false;
    }

    public void DestroyLevelBoundary(int Level = 0)
    {
        if (Level == 0)
        {
            LevelBoundary levelBoundary = GameObject.Find("LevelBoundary").GetComponent<LevelBoundary>();
            levelBoundary.DestroyLevelBoundary();
        }
        else
        {
            foreach (LevelBoundary levelBoundary in m_LevelBoundaryList)
            {
                if (levelBoundary.m_Level == m_PlayerTrans.GetComponent<PlayerProperty>().GetPlayerStat().m_Level)
                {
                    levelBoundary.DestroyLevelBoundary();
                    return;
                }
            }
        }
    }

    public void RespawnObjects(int treeAmount, int rockAmount)
    {
        if (m_SceneName != "TutorialScene")
            return;

        TreeBoundary treeBoundary = GameObject.Find("TreeBoundary").GetComponent<TreeBoundary>();
        treeBoundary.RespawnObjects(treeAmount, rockAmount);
    }

    public void RespawnTrees(int amount)
    {
        if (m_SceneName != "TutorialScene")
            return;

        TreeBoundary treeBoundary = GameObject.Find("TreeBoundary").GetComponent<TreeBoundary>();
        treeBoundary.RespawnObjects(amount, 0);
    }

    public void RespawnRocks(int amount)
    {
        if (m_SceneName != "TutorialScene")
            return;

        TreeBoundary treeBoundary = GameObject.Find("TreeBoundary").GetComponent<TreeBoundary>();
        treeBoundary.RespawnObjects(0, amount);
    }

    public static void SetOriginOpaque(Material material, Color originColor)
    {
        material.SetFloat("_Mode", 0f);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        material.SetInt("_ZWrite", 1);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = -1;

        material.color = originColor;
    }

    public static void SetTransparent(Material material, Color originColor, float alpha)
    {
        material.SetFloat("_Mode", 3f);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
        
        material.color = new Color(originColor.r, originColor.g, originColor.b, alpha);
    }

    public static void SetPingPongTransparent(Material material, float r, float g, float b, float timeSpeed)
    {
        material.SetFloat("_Mode", 3f);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;

        m_PingpongColor = Color.Lerp(new Color(r, g, b, 0), new Color(r, g, b, 1f), Mathf.PingPong(Time.time, timeSpeed));
        material.color = m_PingpongColor;
    }
}
