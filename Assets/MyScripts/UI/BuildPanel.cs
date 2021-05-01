using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuildPanel : MonoBehaviour
{
    public Building.BuildingInfo m_BuildingInfo;
    [System.Serializable]
    public struct BuildPanelInfo
    {
        // 외부에서 입력받을 값
        public Sprite BuildingImage;
        public string BuildingName;
        public string BuildingContext;

        public Sprite UpgradeImage;
        public GameObject Building;
    };
    public BuildPanelInfo m_PanelInfo;

    //
    private GameObject m_Player;
    private GameObject m_BuildingArm;
    private GameObject m_Building;

    private PlayerProperty m_PlayerProperty;
    private UIManager m_UIManager;
    private Building m_BuildingScript;

    // 실제 변수
    private Image m_BuildingImage;
    private Text m_BuildingName;
    private Text m_BuildingContext;

    private Text m_BuildMaximum;
    private Color m_TextColor;

    private Image m_UpgradeImage;
    private Text m_UpgradeMaximum;

    private Button m_BuildButton;
    private Button m_UpgradeButton;

    // 바꿔줄 Text 값
    private int m_BuildingAmount;
    private int m_UpgradeAmount;
    private int m_BuildTime;
    private int m_BuildOriginTime;

    // Start is called before the first frame update
    void Start()
    {
        InitializeUI();
        InitializeValues();
    }

    // Update is called once per frame
    void Update()
    {
        CheckToBuild();
        CheckToUpgrade();
    }

    private void InitializeUI()
    {
        m_BuildingImage = transform.Find("BuildingImage").GetComponent<Image>();
        m_BuildingImage.sprite = m_PanelInfo.BuildingImage;

        m_BuildingName = transform.Find("BuildingName").GetComponent<Text>();
        m_BuildingName.text = m_PanelInfo.BuildingName;

        m_BuildingContext = transform.Find("BuildingContext").GetComponent<Text>();
        m_BuildingContext.text = m_BuildingInfo.m_BuildTime + m_PanelInfo.BuildingContext;

        m_BuildMaximum = transform.Find("BuildMaximum").GetComponent<Text>();
        m_BuildMaximum.text = "0 / " + m_BuildingInfo.m_BuildAmount;

        m_TextColor = m_BuildMaximum.color;

        m_UpgradeImage = transform.Find("UpgradeImage").GetComponent<Image>();
        m_UpgradeImage.sprite = m_PanelInfo.UpgradeImage;

        m_UpgradeMaximum = transform.Find("UpgradeMaximum").GetComponent<Text>();
        m_UpgradeMaximum.text = "0 / " + m_BuildingInfo.m_UpgradeAmount;
        m_UpgradeMaximum.color = Color.red;

        m_BuildButton = transform.Find("BuildButton").GetComponent<Button>();
        m_UpgradeButton = transform.Find("UpgradeButton").GetComponent<Button>();
    }

    private void InitializeValues()
    {
        m_UpgradeAmount = 2;
        m_BuildTime = (int)m_BuildingInfo.m_BuildTime;
        m_BuildOriginTime = m_BuildTime;

        m_Player = GameObject.Find("Player");
        m_PlayerProperty = m_Player.GetComponent<PlayerProperty>();
        m_BuildingArm = m_Player.transform.Find("BuildingArm").gameObject;

        m_UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
    }

    private void CheckToBuild()
    {
        if (m_Building)
            return;

        m_BuildingAmount = m_PlayerProperty.GetPropertyAmount((PlayerProperty.OBJTYPE)m_BuildingInfo.m_eBuildingType);

        if (m_BuildingAmount < m_BuildingInfo.m_BuildAmount)
        {
            m_BuildMaximum.color = Color.red;
            m_BuildButton.interactable = false;
        }
        else
        {
            m_BuildMaximum.color = m_TextColor;
            m_BuildButton.interactable = true;
        }

        m_BuildMaximum.text = m_BuildingAmount + " / " + m_BuildingInfo.m_BuildAmount;
    }

    private void CheckToUpgrade()
    {
        // 빌딩이 클릭되었는지 확인
        ClickTheBuilding();

        // 빌딩 선택이 안된 경우
        if (!m_Building)
            m_UpgradeButton.interactable = false;
        // 선택된 경우
        else
            ClickedBuildingSetting();

        m_UpgradeAmount = m_PlayerProperty.GetPropertyAmount((PlayerProperty.OBJTYPE)(m_BuildingInfo.m_eBuildingType + 2));

        if (m_UpgradeAmount < m_BuildingInfo.m_UpgradeAmount)
        {
            m_UpgradeMaximum.color = Color.red;
            m_UpgradeButton.interactable = false;
        }
        else
            m_UpgradeMaximum.color = m_TextColor;

        m_UpgradeMaximum.text = m_UpgradeAmount + " / " + m_BuildingInfo.m_UpgradeAmount;
    }

    private void ClickedBuildingSetting()
    {
        Building BuildingScript = m_Building.GetComponent<Building>();

        // 빌딩타입이 일치하면
        if (m_BuildingInfo.m_eBuildingType == BuildingScript.GetBuildingInfo().m_eBuildingType)
        {
            m_BuildingContext.text = BuildingScript.GetBuildingInfo().m_BuildTime + m_PanelInfo.BuildingContext;
            m_BuildButton.interactable = true;
            if (m_UpgradeAmount >= m_BuildingInfo.m_UpgradeAmount)
                m_UpgradeButton.interactable = true;

            m_BuildButton.transform.Find("Text").GetComponent<Text>().text = "재배치";
        }
        // 일치 안하면
        else
            m_UpgradeButton.interactable = false;
    }

    private void ClickTheBuilding()
    {
        if (Input.GetMouseButton(0))
        {
            // UI 클릭한 경우 true
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            RaycastHit Hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out Hit, 100, ~(1 << LayerMask.NameToLayer("Player"))))
            {
                if (Hit.collider.gameObject.GetComponent<Building>())
                {
                    if (m_Building)
                        m_BuildingScript.ResetToOrigin();

                    m_Building = Hit.collider.gameObject;
                    m_BuildingScript = m_Building.GetComponent<Building>();

                    m_BuildTime = (int)m_BuildingScript.GetBuildingInfo().m_BuildTime;
                    m_BuildingScript.ClickedToUpdate();

                    m_UIManager.SetReturnIconActive(false);
                }
                else
                    ResetUI();
            }
        }
    }

    public void ClickBuildButton()
    {
        // 빌드
        if (!m_Building /*|| m_Building.transform.parent*/)
        {
            m_Building = Instantiate(m_PanelInfo.Building, new Vector3(0f, 0f, 0f), new Quaternion(0f, 0f, 0f, 0f));
            m_Building.transform.SetParent(m_BuildingArm.transform);

            Building BuildingScript = m_Building.GetComponent<Building>();

            BuildingScript.SetBuildingInfo(m_BuildingInfo);
            m_PlayerProperty.ReduceProperty((PlayerProperty.OBJTYPE)m_BuildingInfo.m_eBuildingType, m_BuildingInfo.m_BuildAmount);

            m_Building = null;
        }
        // 재배치
        else
        {
            m_Building.transform.SetParent(m_BuildingArm.transform);
            m_Building.GetComponent<Building>().SetBuild(global::Building.BUILD.BUILDING);

            ResetUI();
        }

        m_UIManager.SetPhoneCanvasActive(false);
        m_UIManager.SetPlayerRebuild(true);
    }

    public void ClickUpgradeButton()
    {
        Building BuildingScript = m_Building.GetComponent<Building>();
        float BuildTime = BuildingScript.GetBuildingInfo().m_BuildTime;

        if (BuildTime - 50 <= 0)
            return;

        Building.BuildingInfo info;
        info = m_BuildingInfo;
        info.m_BuildTime = BuildTime - 50f;

        m_PlayerProperty.ReduceProperty((PlayerProperty.OBJTYPE)(m_BuildingInfo.m_eBuildingType + 2), m_BuildingInfo.m_UpgradeAmount);
        m_PlayerProperty.AddExperience(m_BuildingInfo.m_Exp);
        m_UpgradeAmount -= m_BuildingInfo.m_UpgradeAmount;
        BuildingScript.SetBuildingInfo(info);

        m_UIManager.SetUpgrade(true);
    }

    private void ResetUI()
    {
        if (m_Building)
            m_BuildingScript.ResetToOrigin();

        m_BuildTime = m_BuildOriginTime;
        m_Building = null;

        m_UpgradeButton.interactable = false;
        m_BuildButton.transform.Find("Text").GetComponent<Text>().text = "건설";
        m_BuildingContext.text = m_BuildTime + m_PanelInfo.BuildingContext;

        m_UIManager.SetReturnIconActive(true);
    }
}
