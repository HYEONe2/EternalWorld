using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingUI : MonoBehaviour
{
    private bool m_bActive;
    private Text m_EndingText;

    private List<string> m_Text = new List<string>();
    private int m_TextCnt;

    private float m_SystemOffTime;

    private GameObject m_Player;
    private GameObject m_EndingCreditUI;
    private GameObject m_UI;

    [SerializeField] private int m_LimitLevel = 0;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    private void OnDestroy()
    {
        m_Text.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_TextCnt >= m_Text.Count)
        {
            if(m_bActive)
                SetActive(false);

            if (m_SystemOffTime > 5f)
            {
                m_UI.SetActive(false);
                m_EndingCreditUI.SetActive(true);
            }
            else
                m_SystemOffTime += Time.deltaTime;
            return;
        }

        if (!m_bActive)
        {
            //if(Input.GetKeyDown(KeyCode.U))
            if (m_Player.GetComponent<PlayerProperty>().GetPlayerStat().m_Level >= m_LimitLevel)
                SetActive(true);
        }
        else
        {
            m_EndingText.text = m_Text[m_TextCnt];

            if (Input.GetKeyDown(KeyCode.Return))
                ++m_TextCnt;
        }
    }

    private void Initialize()
    {
        SetActive(false);
        m_EndingText = transform.Find("EndingText").GetComponent<Text>();

        m_Text.Add("현재 Player님이 게임을 플레이한 지 48시간 0분 지났습니다.");
        m_Text.Add("저희 게임은 Player님의 안전을 위해 자동 종료 시스템을 갖추고 있습니다.");
        m_Text.Add("게임 내의 시간이 현실 시간보다 느리게 흐르므로 유의해주시기 바랍니다.");
        m_Text.Add("지금부터 Player님의 휴식을 위해 5초 후에 게임이 자동 종료 되겠습니다.");
        m_Text.Add("충분한 휴식 후, 다시 방문해주십시오. 감사합니다.");
        m_TextCnt = 0;

        m_Player = GameObject.FindWithTag("Player");
        m_UI = GameObject.Find("UI");
        m_EndingCreditUI = transform.parent.GetChild(1).gameObject;
        m_EndingCreditUI.SetActive(false);
    }

    private void SetActive(bool active)
    {
        for (int i = 0; i < transform.childCount; ++i)
            transform.GetChild(i).gameObject.SetActive(active);

        m_bActive = active;
    }
}
