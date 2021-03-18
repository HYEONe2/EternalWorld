using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildPanel : MonoBehaviour
{
    //
    private enum BUILDING { BUILDING_WOOD, BUILDING_STONE, BUILDING_END };
    private BUILDING m_eBuilding;

    //
    private GameObject m_Player;
    private GameObject m_BuildingArm;
    private GameObject m_Building;

    private PlayerProperty m_PlayerProperty;
    private UIManager m_UIManager;

    // 외부에서 입력받을 값
    public Sprite BuildingImage;
    public string BuildingName;
    public string BuildingContext;

    public Sprite UpgradeImage;
    public GameObject Building;

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
    public int m_MaxBuildingAmount;

    private int m_UpgradeAmount;
    public int m_MaxUpgradeAmount;

    public int m_BuildTime;

    // Start is called before the first frame update
    void Start()
    {
        InitializeUI();
        InitializeValues();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActiveAndEnabled)
            return;

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
    }

    private void InitializeValues()
    {
        if (BuildingName == "통나무집")
            m_eBuilding = BUILDING.BUILDING_WOOD;
        else if (BuildingName == "돌집")
            m_eBuilding = BUILDING.BUILDING_STONE;

        m_Player = GameObject.Find("Player");
        m_PlayerProperty = m_Player.GetComponent<PlayerProperty>();
        m_BuildingArm = m_Player.transform.Find("BuildingArm").gameObject;

        m_UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
    }

    private void CheckToBuild()
    {
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

        if (!m_Building)
        {
            m_UpgradeAmount = 0;
        }
        else
        {
            // 빌딩타입이 일치하면
            if (m_eBuilding == (BuildPanel.BUILDING)m_Building.GetComponent<Building>().GetBuildingType())
            {
                //m_UpgradeAmount = m_PlayerProperty.GetPropertyAmount((PlayerProperty.OBJTYPE)m_eBuilding);
                m_UpgradeAmount = 1;
            }
            else
            {
                m_UpgradeAmount = 0;
            }
        }

        if (m_UpgradeAmount < m_MaxUpgradeAmount)
        {
            m_UpgradeMaximum.color = Color.red;
            m_UpgradeButton.interactable = false;
        }
        else
        {
            m_UpgradeMaximum.color = m_TextColor;
            m_UpgradeButton.interactable = true;
        }

        m_UpgradeMaximum.text = m_UpgradeAmount + " / " + m_MaxUpgradeAmount;
    }

    private void ClickTheBuilding()
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit Hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out Hit, 100))
            {
                if (Hit.collider.gameObject.GetComponent<Building>())
                {
                    m_Building = Hit.collider.gameObject;
                    m_BuildTime = (int)m_Building.GetComponent<Building>().GetBuildTime();
                    // m_BuildTime을 각 Reward마다 설정해주지 않으면 모두 같은 m_BuildTime으로 시간이 바뀜
                    //m_BuildingContext.text = m_BuildTime + BuildingContext;
                }
                else
                {
                    m_Building = null;
                    m_BuildTime = 10;
                    //m_BuildingContext.text = m_BuildTime + BuildingContext;
                }
            }
        }
    }

    public void ClickBuildButton()
    {
        m_Building = Instantiate(Building, new Vector3(0f, 0f, 0f), new Quaternion(0f, 0f, 0f, 0f));
        m_Building.transform.SetParent(m_BuildingArm.transform);

        Building BuildingScript = m_Building.GetComponent<Building>();

        BuildingScript.SetBuildingType((Building.BUILDING)m_eBuilding);
        BuildingScript.SetBuildTime(m_BuildTime);
        BuildingScript.SetBuildAmount(m_MaxBuildingAmount);
        BuildingScript.SetUpgradeAmount(m_MaxUpgradeAmount);

        m_UIManager.SetPhoneCanvasActive(false);

        Cursor.visible = false;
        m_Building = null;
    }

    public void ClickUpgradeButton()
    {
        Building BuildingScript = m_Building.GetComponent<Building>();
        float BuildTime = BuildingScript.GetBuildTime();

        if (BuildTime - 9 <= 0)
            return;

        BuildingScript.SetBuildTime(BuildTime-9);
        m_UpgradeAmount -= m_MaxUpgradeAmount;
    }
}
