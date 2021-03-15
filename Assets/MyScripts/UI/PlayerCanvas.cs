using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCanvas : MonoBehaviour
{
    private GameObject m_NoticeUI;
    private Transform m_NoticeUITrans;

    private bool m_bUseNoticeUI;
    private bool m_bCheckTime;
    private float m_CheckTime;

    // Function
    public void SetUseNoticeUI(bool bUse) { m_bUseNoticeUI = bUse; }

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(Screen.width, (Screen.width * 16) / 9, false);

        if (!m_NoticeUI) m_NoticeUI = GameObject.Find("NoticeUI");
        if (!m_NoticeUITrans) m_NoticeUITrans = m_NoticeUI.transform;

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
