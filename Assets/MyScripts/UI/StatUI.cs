using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatUI : MonoBehaviour
{
    private Image m_ExpImage;
    private Text m_LevelText;
    private Text m_ExpText;

    private PlayerProperty m_PlayerProperty;
    private int m_Level;
    private int m_Exp;
    private int m_MaxExp;

    private bool m_bLateInit;

    // Start is called before the first frame update
    void Start()
    {
        m_ExpImage = transform.Find("ExpImage").GetComponent<Image>();
        m_LevelText = transform.Find("LevelText").GetComponent<Text>();
        m_ExpText = transform.Find("ExpText").GetComponent<Text>();

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
        m_Level = m_PlayerProperty.GetLevel();
        m_Exp = m_PlayerProperty.GetExp();
        m_MaxExp = m_PlayerProperty.GetMaxExp() * m_Level;

        m_ExpImage.fillAmount = (float)m_Exp / (float)m_MaxExp;
        m_LevelText.text = m_Level+"";
        m_ExpText.text = m_Exp + " / " + m_MaxExp;

        m_bLateInit = true;
    }

    private void UpdateStat()
    {
        m_Level = m_PlayerProperty.GetLevel();
        m_Exp = m_PlayerProperty.GetExp();
        m_MaxExp = m_PlayerProperty.GetMaxExp() * m_Level;

        m_LevelText.text = m_Level + "";
        m_ExpText.text = m_Exp + " / " + m_MaxExp;
        m_ExpImage.fillAmount = (float)m_Exp / (float)m_MaxExp;
    }
}
