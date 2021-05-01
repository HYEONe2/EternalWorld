using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour
{
    private Image m_FirstSkill;
    private Image m_SecondSkill;

    private bool m_bLateInit;
    private ObjectManager.ABILITY m_eAbility;

    private GameObject m_Player;
    private Player m_PlayerScript;

    // Start is called before the first frame update
    void Start()
    {
        m_FirstSkill = transform.GetChild(0).GetComponent<Image>();
        m_SecondSkill = transform.GetChild(1).GetComponent<Image>();

        m_FirstSkill.gameObject.SetActive(false);
        m_SecondSkill.gameObject.SetActive(false);

        m_bLateInit = false;
        m_eAbility = ObjectManager.ABILITY.ABIL_END;

        m_Player = GameObject.Find("Player");
        m_PlayerScript = m_Player.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_bLateInit)
            LateInit();
        else
        {
            // 스킬 시전했는지 검사
            if(m_PlayerScript.GetSkillOn(0))
            {
                m_FirstSkill.fillAmount = m_PlayerScript.GetCoolTime(0);
                m_FirstSkill.gameObject.SetActive(true);
            }
            else
            {
                m_FirstSkill.fillAmount = 0;
                m_FirstSkill.gameObject.SetActive(false);
            }

            if (m_PlayerScript.GetSkillOn(1))
            {
                m_SecondSkill.fillAmount = m_PlayerScript.GetCoolTime(1);
                m_SecondSkill.gameObject.SetActive(true);
            }
            else
            {
                m_SecondSkill.fillAmount = 0;
                m_SecondSkill.gameObject.SetActive(false);
            }
        }
    }

    private void LateInit()
    {
        m_eAbility = m_PlayerScript.GetAbility();

        if (m_eAbility != ObjectManager.ABILITY.ABIL_END)
        {
            m_bLateInit = true;

            switch (m_eAbility)
            {
                case ObjectManager.ABILITY.ABIL_FIRE:
                    m_FirstSkill.sprite = Resources.Load<Sprite>("UI/PlayerCanvas/RedCircle");
                    m_SecondSkill.sprite = Resources.Load<Sprite>("UI/PlayerCanvas/RedCircle");
                    break;
                case ObjectManager.ABILITY.ABIL_GRASS:
                    m_FirstSkill.sprite = Resources.Load<Sprite>("UI/PlayerCanvas/GreenCircle");
                    m_SecondSkill.sprite = Resources.Load<Sprite>("UI/PlayerCanvas/GreenCircle");
                    break;
                case ObjectManager.ABILITY.ABIL_WATER:
                    m_FirstSkill.sprite = Resources.Load<Sprite>("UI/PlayerCanvas/BlueCircle");
                    m_SecondSkill.sprite = Resources.Load<Sprite>("UI/PlayerCanvas/BlueCircle");
                    break;
            }
        }
    }
}
