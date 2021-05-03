using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatUI : MonoBehaviour
{
    private Image m_ExpImage;
    private Text m_LevelText;
    private Text m_ExpText;
    private Text m_StrText;
    private Text m_CoolTimeText;

    private PlayerProperty m_PlayerProperty;
    private int m_Level;
    private int m_Exp;
    private int m_MaxExp;
    private int m_Str;
    private int m_CoolTime;

    private bool m_bLateInit;

    // Start is called before the first frame update
    void Start()
    {
        m_ExpImage = transform.Find("ExpImage").GetComponent<Image>();
        m_LevelText = transform.Find("LevelText").GetComponent<Text>();
        m_ExpText = transform.Find("ExpText").GetComponent<Text>();

        m_StrText = transform.Find("StrImage").Find("StrText").GetComponent<Text>();
        m_CoolTimeText = transform.Find("CoolTimeImage").Find("CoolTimeText").GetComponent<Text>();

        m_PlayerProperty = GameObject.Find("Player").GetComponent<PlayerProperty>();
        m_bLateInit = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_bLateInit)
            LateInit();

        UpdateStat();
    }

    private void LateInit()
    {
        PlayerProperty.PlayerStat stat = m_PlayerProperty.GetPlayerStat();

        m_Level = stat.m_Level;
        m_Exp = stat.m_Exp;
        m_MaxExp = stat.m_MaxExp * m_Level;

        m_ExpImage.fillAmount = (float)m_Exp / (float)m_MaxExp;
        m_LevelText.text = m_Level + "";
        m_ExpText.text = m_Exp + " / " + m_MaxExp;

        m_bLateInit = true;
    }

    private void UpdateStat()
    {
        PlayerProperty.PlayerStat stat = m_PlayerProperty.GetPlayerStat();

        m_Level = stat.m_Level;
        m_Exp = stat.m_Exp;
        m_MaxExp = stat.m_MaxExp * m_Level;

        m_LevelText.text = m_Level + "";
        m_ExpText.text = m_Exp + " / " + m_MaxExp;
        m_ExpImage.fillAmount = (float)m_Exp / (float)m_MaxExp;

        m_Str = stat.m_Str;
        m_CoolTime = stat.m_CoolTime;

        m_StrText.text = m_Str + "";
        m_CoolTimeText.text = m_CoolTime + "%";
    }
}
