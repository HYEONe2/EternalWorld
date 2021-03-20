using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameObject m_PlayerCanvas;
    private GameObject m_PhoneCanvas;

    private GameObject m_Menu;
    private GameObject m_Inventory;
    private GameObject m_Shop;
    private GameObject m_Build;

    private GameObject m_ReturnIcon;
    private GameObject m_CoinIcon;

    private PlayerProperty m_PlayerProperty;
    private bool m_bTurnAlpha;
    private float m_PingPongTime;

    public GameObject GetCoinIcon() { return m_CoinIcon; }
    public ShopUI GetShopUIScript() { return m_Shop.GetComponent<ShopUI>(); }
    public PlayerProperty GetPlayerProperty() { return m_PlayerProperty; }

    // Start is called before the first frame update
    void Start()
    {
        if (!m_PlayerCanvas) m_PlayerCanvas = GameObject.Find("PlayerCanvas");
        if (!m_PhoneCanvas) m_PhoneCanvas = GameObject.Find("PhoneCanvas");

        if (!m_Menu) m_Menu = GameObject.Find("MenuUI");
        if (!m_Inventory) m_Inventory = GameObject.Find("InventoryUI");
        if (!m_Shop) m_Shop = GameObject.Find("ShopUI");
        if (!m_Build) m_Build = GameObject.Find("BuildUI");

        if (!m_ReturnIcon) m_ReturnIcon = GameObject.Find("ReturnIcon");
        if (!m_CoinIcon) m_CoinIcon = GameObject.Find("CoinIcon");

        m_PlayerProperty = GameObject.Find("Player").GetComponent<PlayerProperty>();

        m_ReturnIcon.SetActive(false);
        m_Inventory.SetActive(false);
        m_Shop.SetActive(false);
        m_Build.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        m_CoinIcon.transform.Find("Text").GetComponent<Text>().text = m_PlayerProperty.GetCoin() +"";
    }

    public void ClickInventory()
    {
        m_Menu.SetActive(false);
        m_ReturnIcon.SetActive(true);

        m_Inventory.SetActive(true);
    }

    public void ClickShop()
    {
        m_Menu.SetActive(false);
        m_ReturnIcon.SetActive(true);

        m_Shop.SetActive(true);
    }

    public void ClickBuild()
    {
        m_Menu.SetActive(false);
        m_ReturnIcon.SetActive(true);

        m_Build.SetActive(true);
    }

    public void ClickSetting()
    {
        m_Menu.SetActive(false);
        m_ReturnIcon.SetActive(true);
    }

    public void ClickBackward()
    {
        if (m_Menu.activeSelf)
        {
            return;
        }
        else
        {
            m_Inventory.SetActive(false);
            m_Shop.SetActive(false);
            m_Build.SetActive(false);

            m_ReturnIcon.SetActive(false);
            m_Menu.SetActive(true);
        }
    }

    public void SetPhoneCanvasActive(bool active)
    {
        m_PhoneCanvas.SetActive(active);
        Cursor.visible = active;
    }

    public void SetNoticeUI(PlayerProperty.OBJTYPE eType, int amount)
    {
        m_PlayerCanvas.transform.Find("NoticeUI").GetComponent<NoticeUI>().SetNoticeUI(eType, amount);
        m_PlayerCanvas.GetComponent<PlayerCanvas>().SetUseNoticeUI(true);
    }

    public void SetReturnIconActive(bool visible)
    {
        m_ReturnIcon.SetActive(visible);
    }

    public void SetPingPongText(Text text, float r, float g, float b)
    {
        if (!m_bTurnAlpha)
        {
            m_PingPongTime += Time.deltaTime * 2f;

            if (m_PingPongTime > 1f)
                m_bTurnAlpha = true;
        }
        else
        {
            m_PingPongTime -= Time.deltaTime * 2f;

            if (m_PingPongTime <= 0f)
                m_bTurnAlpha = false;
        }


        text.color = new Color(r, g, b, m_PingPongTime);
    }

    public void SetTextColor(Text text, float r, float g, float b, float a)
    {
        text.color = new Color(r, g, b, a);
    }

    public bool GetActive(GameObject tempObject)
    {
        return tempObject.activeSelf;
    }
}
