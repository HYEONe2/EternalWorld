using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectManager : MonoBehaviour
{
    private string m_SceneName;
    private Transform m_PlayerTrans;

    private List<LevelBoundary> m_LevelBoundaryList = new List<LevelBoundary>();
    private List<TreeBoundary> m_TreeBoundaryList = new List<TreeBoundary>();

    public string GetSceneName() { return m_SceneName; }

    // Start is called before the first frame update
    void Start()
    {
        m_SceneName = SceneManager.GetActiveScene().name;
        m_PlayerTrans = GameObject.Find("Player").transform;

        if (m_SceneName != "TutorialScene")
        {
            InitializeBoundary();
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScene();
        UpdateBoundary();
    }

    private void InitializeBoundary()
    {
        GameObject[] TempBoundary = GameObject.FindGameObjectsWithTag("LevelBoundary");
        foreach (GameObject iter in TempBoundary)
        {
            m_LevelBoundaryList.Add(iter.GetComponent<LevelBoundary>());
        }

        TempBoundary = GameObject.FindGameObjectsWithTag("TreeBoundary");
        foreach (GameObject iter in TempBoundary)
        {
            m_TreeBoundaryList.Add(iter.GetComponent<TreeBoundary>());
        }
    }

    private void UpdateScene()
    {
        if (m_SceneName == SceneManager.GetActiveScene().name)
            return;

        m_SceneName = SceneManager.GetActiveScene().name;

        m_LevelBoundaryList.Clear();
        m_TreeBoundaryList.Clear();
    }

    private void UpdateBoundary()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TreeBoundary treeBoundary = GameObject.Find("TreeBoundary").GetComponent<TreeBoundary>();
            RespawnTrees(3);
            RespawnRocks(3);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            LevelBoundary levelBoundary = GameObject.Find("LevelBoundary").GetComponent<LevelBoundary>();
            levelBoundary.DestroyLevelBoundary();
            return;
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
            foreach(LevelBoundary levelBoundary in m_LevelBoundaryList)
            {
                if(levelBoundary.m_Level == m_PlayerTrans.GetComponent<PlayerProperty>().GetLevel())
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
}
