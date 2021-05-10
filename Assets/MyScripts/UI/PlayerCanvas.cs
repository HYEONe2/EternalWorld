using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCanvas : MonoBehaviour
{
    private GameObject m_NoticeUI;
    private Transform m_NoticeUITrans;

    private GameObject m_MainQuestUI;

    private bool m_bUseNoticeUI;
    private bool m_bCheckTime;
    private float m_CheckTime;

    // Function
    public void SetUseNoticeUI(bool bUse) { m_bUseNoticeUI = bUse; }
    public void SetMainQuestUIActive(bool bActive) { if(m_MainQuestUI) m_MainQuestUI.SetActive(bActive); }

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(Screen.width, (Screen.width * 16) / 9, false);

        if (!m_NoticeUITrans) m_NoticeUITrans = transform.Find("NoticeUI");
        if (!m_NoticeUI) m_NoticeUI = m_NoticeUITrans.gameObject;

        m_MainQuestUI = transform.Find("MainQuestUI").gameObject;
        m_MainQuestUI.SetActive(false);

        m_bUseNoticeUI = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_bCheckTime)
        {
            if (m_CheckTime > 5f)
            {
                NoticeUIGoingLeft();
                return;
            }

            m_CheckTime += Time.deltaTime;
        }
        else
        {
            if (m_bUseNoticeUI)
                NoticeUIGoingRight();
        }
    }

    void NoticeUIGoingRight()
    {
        if (m_NoticeUITrans.position.x >= 120f)
        {
            m_bCheckTime = true;
            return;
        }

        m_NoticeUITrans.position += new Vector3(500f * Time.deltaTime, 0f, 0f);
    }

    void NoticeUIGoingLeft()
    {
        if (m_NoticeUITrans.position.x <= -120f)
        {
            m_bCheckTime = false;
            m_CheckTime = 0f;
            m_bUseNoticeUI = false;
            return;
        }

        m_NoticeUITrans.position -= new Vector3(500f * Time.deltaTime, 0f, 0f);
    }
}
