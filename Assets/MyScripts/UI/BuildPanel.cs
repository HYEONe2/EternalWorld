using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildPanel : MonoBehaviour
{
    //
    private enum BUILDING { BUILDING_WOOD, BUILDING_STONE, BUILDING_END};
    private BUILDING m_eBuilding;
    private bool m_bBuilding;

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
        CheckBuilding();
        CheckToUpgrade();
    }

    private void InitializeUI()
    {
        m_BuildingImage = transform.Find("BuildingImage").GetComponent<Image>();
        m_BuildingImage.sprite = BuildingImage;

        m_BuildingName = transform.Find("BuildingName").GetComponent<Text>();
        m_BuildingName.text = BuildingName;

        m_BuildingContext = transform.Find("BuildingContext").GetComponent<Text>();
        m_BuildingContext.text = BuildingContext;

        m_BuildMaximum = transform.Find("BuildMaximum").GetComponent<Text>();
        m_BuildMaximum.text = "0 / " + m_MaxBuildingAmount;

        m_TextColor = m_BuildMaximum.color;

        m_UpgradeImage = transform.Find("UpgradeImage").GetComponent<Image>();
        m_UpgradeImage.sprite = UpgradeImage;

        m_UpgradeMaximum = transform.Find("UpgradeMaximum").GetComponent<Text>();
        m_UpgradeMaximum.text = "0 / " + m_MaxUpgradeAmount;

        m_BuildButton = transform.Find("BuildButton").GetComponent<Button>();
        m_UpgradeButton = transform.Find("UpgradeButton").GetComponent<Button>();
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
        m_bBuilding = false;
    }

    private void CheckToBuild()
    {
        switch(m_eBuilding)
        {
            case BUILDING.BUILDING_WOOD:
                {
                    m_BuildingAmount = m_PlayerProperty.GetPropertyAmount(PlayerProperty.OBJTYPE.OBJ_WOOD);

                    if (m_BuildingAmount < m_MaxBuildingAmount)
                    {
                        m_BuildMaximum.color = Color.red;
                        m_BuildButton.interactable = false;
                    }
                    else
                    {
                        m_BuildMaximum.color = m_TextColor;
                        m_BuildButton.interactable = true;

                        m_BuildMaximum.text = m_BuildingAmount + " / " + m_MaxBuildingAmount;
                    }
                }
                break;
            case BUILDING.BUILDING_STONE:
                {
                    m_BuildingAmount = m_PlayerProperty.GetPropertyAmount(PlayerProperty.OBJTYPE.OBJ_STONE);

                    if (m_BuildingAmount < m_MaxBuildingAmount)
                    {
                        m_BuildMaximum.color = Color.red;
                        m_BuildButton.interactable = false;
                    }
                    else
                    {
                        m_BuildMaximum.color = m_TextColor;
                        m_BuildButton.interactable = true;

                        m_BuildMaximum.text = m_BuildingAmount + " / " + m_MaxBuildingAmount;
                    }
                }
                break;
        }
    }

    private void CheckToUpgrade()
    {
        switch (m_eBuilding)
        {
            case BUILDING.BUILDING_WOOD:
                {
                    if (m_UpgradeAmount < m_MaxUpgradeAmount)
                    {
                        m_UpgradeMaximum.color = Color.red;
                        m_UpgradeButton.interactable = false;
                    }
                    else
                    {
                        m_UpgradeMaximum.color = m_TextColor;
                        m_UpgradeButton.interactable = true;

                        m_UpgradeMaximum.text = m_UpgradeAmount + " / " + m_MaxUpgradeAmount;
                    }
                }
                break;
            case BUILDING.BUILDING_STONE:
                {
                    if (m_UpgradeAmount < m_MaxUpgradeAmount)
                    {
                        m_UpgradeMaximum.color = Color.red;
                        m_UpgradeButton.interactable = false;
                    }
                    else
                    {
                        m_UpgradeMaximum.color = m_TextColor;
                        m_UpgradeButton.interactable = true;

                        m_UpgradeMaximum.text = m_UpgradeAmount + " / " + m_MaxUpgradeAmount;
                    }
                }
                break;
        }
    }

    private void CheckBuilding()
    {
        if (!m_Building)
            m_bBuilding = false;

        m_UIManager.SetPhoneCanvasBuilding(m_bBuilding);
    }

    public void ClickBuildButton()
    {
        m_Building = Instantiate(Building, new Vector3(0f, 0f, 0f), new Quaternion(0f, 0f, 0f, 0f));
        m_Building.transform.SetParent(m_BuildingArm.transform);

        m_bBuilding = true;
        m_UIManager.SetPhoneCanvasActive(false);

        Cursor.visible = false;
    }

    public void ClickUpgradeButton()
    {

    }
}
