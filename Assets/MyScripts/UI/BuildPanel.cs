using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuildPanel : MonoBehaviour
{
    //
    private GameObject m_Player;
    private GameObject m_BuildingArm;
    private GameObject m_Building;

    private PlayerProperty m_PlayerProperty;
    private UIManager m_UIManager;
    private Building m_BuildingScript;

    // 외부에서 입력받을 값
    public Sprite BuildingImage;
    public string BuildingName;
    public string BuildingContext;

    public Sprite UpgradeImage;
    public GameObject Building;

    // 실제 변수
    public Building.BUILDING m_eBuilding;

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
    public int m_MaxBuildingAmount;

    private int m_UpgradeAmount;
    public int m_MaxUpgradeAmount;

    public int m_BuildTime;
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
        m_BuildingImage.sprite = BuildingImage;

        m_BuildingName = transform.Find("BuildingName").GetComponent<Text>();
        m_BuildingName.text = BuildingName;

        m_BuildingContext = transform.Find("BuildingContext").GetComponent<Text>();
        m_BuildingContext.text = m_BuildTime + BuildingContext;

        m_BuildMaximum = transform.Find("BuildMaximum").GetComponent<Text>();
        m_BuildMaximum.text = "0 / " + m_MaxBuildingAmount;

        m_TextColor = m_BuildMaximum.color;

        m_UpgradeImage = transform.Find("UpgradeImage").GetComponent<Image>();
        m_UpgradeImage.sprite = UpgradeImage;

        m_UpgradeMaximum = transform.Find("UpgradeMaximum").GetComponent<Text>();
        m_UpgradeMaximum.text = "0 / " + m_MaxUpgradeAmount;
        m_UpgradeMaximum.color = Color.red;

        m_BuildButton = transform.Find("BuildButton").GetComponent<Button>();
        m_UpgradeButton = transform.Find("UpgradeButton").GetComponent<Button>();
        m_UpgradeAmount = 2;
        m_BuildOriginTime = m_BuildTime;
    }

    private void InitializeValues()
    {
        m_Player = GameObject.Find("Player");
        m_PlayerProperty = m_Player.GetComponent<PlayerProperty>();
        m_BuildingArm = m_Player.transform.Find("BuildingArm").gameObject;

        m_UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
    }

    private void CheckToBuild()
    {
        if (m_Building)
            return;

        m_BuildingAmount = m_PlayerProperty.GetPropertyAmount((PlayerProperty.OBJTYPE)m_eBuilding);

        if (m_BuildingAmount < m_MaxBuildingAmount)
        {
            m_BuildMaximum.color = Color.red;
            m_BuildButton.interactable = false;
        }
        else
        {
            m_BuildMaximum.color = m_TextColor;
            m_BuildButton.interactable = true;
        }

        m_BuildMaximum.text = m_BuildingAmount + " / " + m_MaxBuildingAmount;
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

        m_UpgradeAmount = m_PlayerProperty.GetPropertyAmount((PlayerProperty.OBJTYPE)(m_eBuilding + 2));

        if (m_UpgradeAmount < m_MaxUpgradeAmount)
        {
            m_UpgradeMaximum.color = Color.red;
            m_UpgradeButton.interactable = false;
        }
        else
            m_UpgradeMaximum.color = m_TextColor;

        m_UpgradeMaximum.text = m_UpgradeAmount + " / " + m_MaxUpgradeAmount;
    }

    private void ClickedBuildingSetting()
    {
        // 빌딩타입이 일치하면
        if (m_eBuilding == m_Building.GetComponent<Building>().GetBuildingType())
        {
            m_BuildingContext.text = m_Building.GetComponent<Building>().GetBuildTime() + BuildingContext;
            m_BuildButton.interactable = true;
            if (m_UpgradeAmount >= m_MaxUpgradeAmount)
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

                    m_BuildTime = (int)m_BuildingScript.GetBuildTime();
                    m_BuildingScript.ClickedToUpdate();

                    m_UIManager.SetReturnIconActive(false);
                }
                else
                    ResetUI();
            }

            m_UIManager.SetRebuild(false);
            m_UIManager.SetUpgrade(false);
        }
    }

    public void ClickBuildButton()
    {
        // 빌드
        if (!m_Building || m_Building.transform.parent)
        {
            m_Building = Instantiate(Building, new Vector3(0f, 0f, 0f), new Quaternion(0f, 0f, 0f, 0f));
            m_Building.transform.SetParent(m_BuildingArm.transform);

            Building BuildingScript = m_Building.GetComponent<Building>();

            BuildingScript.SetBuildingType((Building.BUILDING)m_eBuilding);
            BuildingScript.SetBuildTime(m_BuildTime);
            BuildingScript.SetBuildAmount(m_MaxBuildingAmount);
            BuildingScript.SetUpgradeAmount(m_MaxUpgradeAmount);
            m_PlayerProperty.ReduceProperty((PlayerProperty.OBJTYPE)m_eBuilding, m_MaxBuildingAmount);

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
        m_UIManager.SetRebuild(true);
    }

    public void ClickUpgradeButton()
    {
        Building BuildingScript = m_Building.GetComponent<Building>();
        float BuildTime = BuildingScript.GetBuildTime();

        if (BuildTime - 9 <= 0)
            return;

        m_PlayerProperty.ReduceProperty((PlayerProperty.OBJTYPE)(m_eBuilding + 2), m_MaxUpgradeAmount);
        BuildingScript.SetBuildTime(BuildTime - 9);
        m_UpgradeAmount -= m_MaxUpgradeAmount;

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
        m_BuildingContext.text = m_BuildTime + BuildingContext;

        m_UIManager.SetReturnIconActive(true);
    }
}
