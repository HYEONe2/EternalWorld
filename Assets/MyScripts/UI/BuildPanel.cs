using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildPanel : MonoBehaviour
{
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

    private GameObject m_Player;
    private PlayerProperty m_PlayerProperty;

    // Start is called before the first frame update
    void Start()
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

        m_Player = GameObject.Find("Player");
        m_PlayerProperty = GameObject.Find("Player").GetComponent<PlayerProperty>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActiveAndEnabled)
            return;

        CheckToBuild();
        CheckToUpgrade();
    }

    private void CheckToBuild()
    {
        if (m_BuildingName.text == "통나무집")
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
        else if (m_BuildingName.text == "돌집")
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
    }

    private void CheckToUpgrade()
    {
        if (m_BuildingName.text == "통나무집")
        {
            if(m_UpgradeAmount < m_MaxUpgradeAmount)
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
        else if (m_BuildingName.text == "돌집")
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
    }

    public void ClickBuildButton()
    {
        Vector3 PlayerPosition = m_Player.transform.position;
        PlayerPosition.y += 5f;
        Vector3 PlayerLook = m_Player.transform.forward;
        PlayerLook.Normalize();

        GameObject building = Instantiate(Building, PlayerPosition + PlayerLook * 5f, new Quaternion(0f, 0f, 0f, 0f));
    }

    public void ClickUpgradeButton()
    {

    }
}
