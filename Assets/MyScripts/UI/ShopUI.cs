using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    private List<ShopPanel> m_ShopPanelList = new List<ShopPanel>();
    private List<Text> m_BuySellTextList = new List<Text>();

    private Button m_ChangeButton;
    private Image m_RewardImage;
    private Image m_FillImage;

    private bool m_bLateInit;
    private bool m_bBuy;

    private UIManager m_UIManager;
    private PlayerProperty.OBJTYPE m_eType;
    private int m_FillGauge;
    private int m_EndGauge;

    // Start is called before the first frame update
    void Start()
    {
        m_ChangeButton = transform.Find("ChangeButton").GetComponent<Button>();
        m_RewardImage = transform.Find("Gauge").Find("RewardImage").GetComponent<Image>();
        m_FillImage = transform.Find("Gauge").Find("Fill").GetComponent<Image>();
        m_FillImage.fillAmount = 0;

        m_bLateInit = false;
        m_bBuy = true;

        m_UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        ReInitGauge(100);
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_bLateInit)
            LateInit();
    }

    private void OnDestroy()
    {
        m_ShopPanelList.Clear();
        m_BuySellTextList.Clear();
    }

    private void LateInit()
    {
        Transform Content = transform.GetChild(0).GetChild(0).GetChild(0);

        for(int i=0; i<Content.childCount; ++i)
        {
            m_ShopPanelList.Add(Content.GetChild(i).GetComponent<ShopPanel>());
            m_BuySellTextList.Add(Content.GetChild(i).Find("BuySellButton").GetChild(0).GetComponent<Text>());
        }

        m_bLateInit = true;
    }

    public void ClickChangeButton()
    {
        // Buy -> 사기 (패널버튼 이름 바뀜)
        if (m_bBuy)
        {
            for (int i = 0; i < m_BuySellTextList.Count; ++i)
                m_BuySellTextList[i].text = "팔기";

            m_bBuy = false;
        }
        // Sell -> 팔기
        else
        {
            for (int i = 0; i < m_BuySellTextList.Count; ++i)
                m_BuySellTextList[i].text = "사기";

            m_bBuy = true;
        }

        for (int i = 0; i < m_ShopPanelList.Count; ++i)
            m_ShopPanelList[i].ClickResetButton();
    }

    private void ReInitGauge(int endGauge)
    {
        m_FillGauge = 0;
        m_EndGauge = endGauge;
        m_FillImage.fillAmount = (float)m_FillGauge / (float)m_EndGauge;

        m_eType = (PlayerProperty.OBJTYPE)Random.Range(2, 4);
        switch(m_eType)
        {
            case PlayerProperty.OBJTYPE.OBJ_REDGEMSTONE:
                m_RewardImage.sprite = Resources.Load<Sprite>("UI/PlayerCanvas/Cut Ruby");
                break;
            case PlayerProperty.OBJTYPE.OBJ_BLUEGEMSTONE:
                m_RewardImage.sprite = Resources.Load<Sprite>("UI/PlayerCanvas/Cut Sapphire");
                break;
        }

        m_UIManager.SetEarnGem(true);
    }

    public void UpdateGauge(int gauge)
    {
        m_FillGauge += gauge;
        m_FillImage.fillAmount = (float)m_FillGauge / (float)m_EndGauge;

        if(m_FillGauge >= m_EndGauge)
        {
            m_UIManager.GetPlayerProperty().AddProperty(m_eType, 1);
            m_UIManager.SetNoticeUI(m_eType, 1);
            ReInitGauge(m_EndGauge + 50);
        }
    }
}
