using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPUI : MonoBehaviour
{
    private Image m_HPImage;
    private Text m_HPText;

    private PlayerProperty m_Player;
    private float m_MaxHP;
    private float m_HP;

    // Start is called before the first frame update
    void Start()
    {
        m_HPImage = transform.Find("HPImage").GetComponent<Image>();
        m_HPText = transform.Find("HPText").GetComponent<Text>();

        m_Player = GameObject.FindWithTag("Player").GetComponent<PlayerProperty>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerProperty.PlayerStat stat = m_Player.GetPlayerStat();

        if (m_HP == stat.m_HP)
            return;

        m_MaxHP = 50 * stat.m_Level;
        m_HP = stat.m_HP;

        m_HPText.text = m_HP + " / " + m_MaxHP;
        m_HPImage.fillAmount = (float)(m_HP / m_MaxHP);
    }
}
