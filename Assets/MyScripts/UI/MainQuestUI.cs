using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MainQuestUI : MonoBehaviour
{
    [Serializable]
    public struct QuestInfo
    {
        public string QuestText;
        public bool HasGoalAmount;
        public int CurrentAmount;
        public int MaxAmount;
    };

    [SerializeField] private QuestInfo[] m_QuestInfo = null;
    private int m_QuestIndex;

    private Image m_BackImage;
    private Text m_QuestText;

    private bool m_bFinishTask;

    private Color m_BackOriginColor;
    private float m_Alpha;

    private bool m_bTurnAlpha;
    private float m_ColorTime;

    private bool m_bSlide;

    /////////////////////////////////////////////////////
    private PlayerProperty m_PlayerProperty;
    private BuildingManager m_BuildingManager;

    private int m_ClearCount = 0;
    private int m_UpgradeCount = 0;
    private int m_LandmarkCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_QuestIndex = 0;

        m_BackImage = transform.GetChild(0).GetComponent<Image>();
        m_QuestText = transform.GetChild(2).GetComponent<Text>();

        m_bFinishTask = false;

        m_BackOriginColor = m_BackImage.color;
        m_Alpha = m_BackOriginColor.a;

        m_bTurnAlpha = false;
        m_ColorTime = 0f;

        m_bSlide = false;

        m_PlayerProperty = GameObject.FindWithTag("Player").GetComponent<PlayerProperty>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_BuildingManager)
        {
            GameObject BuilidngManager = GameObject.Find("BuildingManager");
            if (BuilidngManager)
                m_BuildingManager = BuilidngManager.GetComponent<BuildingManager>();
        }

        if (m_QuestIndex >= m_QuestInfo.Length)
        {
            Destroy(gameObject);
            return;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            m_bFinishTask = true;
        }

        // 슬라이드
        if (Input.GetKeyDown(KeyCode.Tab))
            m_bSlide = !m_bSlide;
        SlideQuestUI();

        // 퀘스트 업데이트
        UpdateQuestText();
        UpdateQuest();

        if (m_bFinishTask)
            ChangeBackColor();
    }

    private void SlideQuestUI()
    {
        Vector3 Pos = transform.position;

        if (m_bSlide)
        {
            if (Pos.x >= 2170f)
                transform.position = new Vector3(2170f, Pos.y, Pos.z);
            else
            {
                Pos.x += 300f * Time.deltaTime;
                transform.position = Pos;
            }
        }
        else
        {
            if (Pos.x <= 1870f)
                transform.position = new Vector3(1870f, Pos.y, Pos.z);
            else
            {
                Pos.x -= 300f * Time.deltaTime;
                transform.position = Pos;
            }
        }
    }

    private void UpdateQuestText()
    {
        if (m_QuestIndex >= m_QuestInfo.Length)
            return;

        QuestInfo tempInfo = m_QuestInfo[m_QuestIndex];
        string tempText = tempInfo.QuestText;

        if (tempInfo.HasGoalAmount)
            tempText += " ( " + tempInfo.CurrentAmount + " / " + tempInfo.MaxAmount + " ) ";

        m_QuestText.text = tempText;
    }

    private void UpdateQuest()
    {
        if (m_QuestIndex >= m_QuestInfo.Length)
            return;

        switch (m_QuestIndex)
        {
            case 0:
                {
                    if (m_PlayerProperty.GetPlayerStat().m_Level == 2)
                        m_bFinishTask = true;
                }
                break;
            case 1:
                {
                    int buildingCount = m_BuildingManager.GetBuildingCount("CopperSmithy(Clone)");
                    if (buildingCount >= 1)
                    {
                        m_QuestInfo[m_QuestIndex].CurrentAmount = buildingCount;
                        m_bFinishTask = true;
                    }

                    m_ClearCount = m_PlayerProperty.GetClearDungeon();
                }
                break;
            case 2:
                {
                    if (m_PlayerProperty.GetClearDungeon() >= m_ClearCount + 1)
                        m_bFinishTask = true;

                    m_UpgradeCount = m_PlayerProperty.GetUpgradeCount();
                }
                break;
            case 3:
                {
                    int UpgradeCount = m_PlayerProperty.GetUpgradeCount();
                    if (UpgradeCount >= m_UpgradeCount + 1)
                    {
                        m_QuestInfo[m_QuestIndex].CurrentAmount = (UpgradeCount - m_UpgradeCount);

                        if ((UpgradeCount - m_UpgradeCount) >= 3)
                            m_bFinishTask = true;
                    }

                    int m_LandmarkCount = m_BuildingManager.GetBuildingCount("HealLandmark(Clone)")
                                        + m_BuildingManager.GetBuildingCount("HPLandmark(Clone)")
                                        + m_BuildingManager.GetBuildingCount("CooltimeLandmark(Clone)")
                                        + m_BuildingManager.GetBuildingCount("StrLandmark(Clone)");
                }
                break;
            case 4:
                {
                    int landmarkCount = m_BuildingManager.GetBuildingCount("HealLandmark(Clone)")
                                        + m_BuildingManager.GetBuildingCount("HPLandmark(Clone)")
                                        + m_BuildingManager.GetBuildingCount("CooltimeLandmark(Clone)")
                                        + m_BuildingManager.GetBuildingCount("StrLandmark(Clone)");

                    if(landmarkCount>=m_LandmarkCount+1)
                    {
                        m_QuestInfo[m_QuestIndex].CurrentAmount = (landmarkCount - m_LandmarkCount);
                        m_bFinishTask = true;
                    }
                }
                break;
        }
    }

    private void ChangeBackColor()
    {
        if (m_ColorTime >= 2f)
        {
            m_BackImage.color = m_BackOriginColor;
            m_ColorTime = 0f;
            m_bTurnAlpha = false;

            m_bFinishTask = false;
            ++m_QuestIndex;
            return;
        }
        else
            m_ColorTime += Time.deltaTime;

        if (m_Alpha <= 0)
            m_bTurnAlpha = true;
        else if (m_Alpha >= 1f)
            m_bTurnAlpha = false;

        if (m_bTurnAlpha)
            m_Alpha += 2f * Time.deltaTime;
        else
            m_Alpha -= 2f * Time.deltaTime;

        m_BackImage.color = new Color(0f, 0f, 1f, m_Alpha);
    }
}
