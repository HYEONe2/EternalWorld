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
        if (m_HP == m_Player.GetHP())
            return;

        m_MaxHP = 50 * m_Player.GetLevel();
        m_HP = m_Player.GetHP();

        m_HPText.text = m_HP + " / " + m_MaxHP;
        m_HPImage.fillAmount = (float)(m_HP / m_MaxHP);
    }
}
