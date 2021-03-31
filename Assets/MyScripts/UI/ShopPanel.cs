using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanel : MonoBehaviour
{
    public PlayerProperty.OBJTYPE m_eType;
    private int m_MyAmount;
    private int m_Amount;
    private int m_Coin;
    public int m_OnePerCoin;
    private int m_OriginCost;

    private Text m_MyAmountText;
    private Text m_AmountText;
    private Text m_CoinText;

    private Button m_MinusButton;
    private Button m_PlusButton;
    private Button m_BuySellButton;
    private Button m_ResetButton;

    private UIManager m_UIManager;
    private PlayerProperty m_PlayerProperty;
    private Text m_MyCoinText;

    private bool m_bBuy;
    private bool[] m_bChangeColor = new bool[2];

    // Start is called before the first frame update
    void Start()
    {
        m_Amount = 1;
        m_OriginCost = m_OnePerCoin;

        m_MyAmountText = transform.Find("MyAmountText").GetComponent<Text>();
        m_AmountText = transform.Find("AmountText").GetComponent<Text>();
        m_CoinText = transform.Find("CoinText").GetComponent<Text>();

        m_MinusButton = transform.Find("MinusButton").GetComponent<Button>();
        m_PlusButton = transform.Find("PlusButton").GetComponent<Button>();
        m_BuySellButton = transform.Find("BuySellButton").GetComponent<Button>();
        m_ResetButton = transform.Find("ResetButton").GetComponent<Button>();

        m_UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        m_PlayerProperty = GameObject.Find("Player").GetComponent<PlayerProperty>();

        m_bBuy = true;
        m_BuySellButton.transform.GetChild(0).GetComponent<Text>().text = "사기";

        m_bChangeColor[0] = false;
        m_bChangeColor[1] = false;
    }

    // Update is called once per frame
    void Update()
    {
        RecheckValues();
        ChangeColor();
    }

    private void RecheckValues()
    {
        m_MyAmount = m_PlayerProperty.GetPropertyAmount(m_eType);
        m_Coin = m_PlayerProperty.GetCoin();

        m_MyAmountText.text = "x" + m_MyAmount;
        m_AmountText.text = m_Amount + "";
        m_CoinText.text = (m_OnePerCoin * m_Amount) + "";

        if (m_BuySellButton.transform.Find("Text").GetComponent<Text>().text == "사기")
        {
            m_OnePerCoin = m_OriginCost;
            m_bBuy = true;
        }
        else if (m_BuySellButton.transform.Find("Text").GetComponent<Text>().text == "팔기")
        {
            m_OnePerCoin = (int)(m_OriginCost * 0.1f);
            m_bBuy = false;
        }
    }

    private void ChangeColor()
    {
        // 0: BuyFail-> Coin 1: SellFail->Amount
        if (m_bChangeColor[0])
        {
            if (!m_MyCoinText)
                m_MyCoinText = m_UIManager.GetCoinIcon().transform.Find("Text").GetComponent<Text>();

            m_UIManager.SetPingPongText(m_CoinText, 1f, 0f, 0f);
            m_UIManager.SetPingPongText(m_AmountText, 1f, 0f, 0f);
        }
        if (m_bChangeColor[1])
        {
            m_UIManager.SetPingPongText(m_MyAmountText, 1f, 0f, 0f);
            m_UIManager.SetPingPongText(m_AmountText, 1f, 0f, 0f);
        }
    }

    public void ClickMinusButton()
    {
        if (m_Amount - 1 < 1)
            return;

        if (m_bChangeColor[0])
        {
            if (m_Coin >= (m_OnePerCoin * (m_Amount - 1)))
            {
                m_UIManager.SetTextColor(m_CoinText, 0.196f, 0.196f, 0.196f, 1f);
                m_UIManager.SetTextColor(m_AmountText, 0.196f, 0.196f, 0.196f, 1f);
                m_bChangeColor[0] = false;
            }
        }
        if (m_bChangeColor[1])
        {
            if (m_MyAmount >= m_Amount - 1)
            {
                m_UIManager.SetTextColor(m_MyAmountText, 1f, 1f, 1f, 1f);
                m_UIManager.SetTextColor(m_AmountText, 0.196f, 0.196f, 0.196f, 1f);
                m_bChangeColor[1] = false;
            }
        }

        m_Amount -= 1;
    }

    public void ClickPlusButton()
    {
        if (m_bChangeColor[0] || m_bChangeColor[1])
            return;

        m_Amount += 1;
    }

    public void ClickBuySellButton()
    {
        if(m_bBuy)
        {
            if (m_Coin < m_OnePerCoin * m_Amount)
            {
                m_bChangeColor[0] = true;
                return;
            }

            m_PlayerProperty.AddProperty(m_eType, m_Amount);
            m_PlayerProperty.SetCoin(m_Coin - (m_OnePerCoin * m_Amount));
            m_PlayerProperty.AddExperience(10 * m_Amount);

            m_UIManager.GetShopUIScript().UpdateGauge(10 * m_Amount);
        }
        else
        {
            if (m_MyAmount < m_Amount)
            {
                m_bChangeColor[1] = true;
                return;
            }

            m_PlayerProperty.ReduceProperty(m_eType, m_Amount);
            m_PlayerProperty.SetCoin(m_Coin + (m_OnePerCoin * m_Amount));
            m_PlayerProperty.AddExperience(10 * m_Amount);

            m_UIManager.GetShopUIScript().UpdateGauge(10 * m_Amount);
        }
    }

    public void ClickResetButton()
    {
        m_Amount = 1;

        m_UIManager.SetTextColor(m_CoinText, 0.196f, 0.196f, 0.196f, 1f);
        m_UIManager.SetTextColor(m_MyAmountText, 1f, 1f, 1f, 1f);
        m_UIManager.SetTextColor(m_AmountText, 0.196f, 0.196f, 0.196f, 1f);

        m_bChangeColor[0] = false;
        m_bChangeColor[1] = false;
    }
}
