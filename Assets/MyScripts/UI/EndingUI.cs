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

            if (m_SystemOffTime > 1f)
                LoadingSceneManager.LoadScene("TitleScene");
            else
                m_SystemOffTime += Time.deltaTime;
            return;
        }

        if (!m_bActive)
        {
            if (m_Player.GetComponent<PlayerProperty>().GetLevel() >= 7)
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
        m_Text.Add("게임 내의 시간이 현실 시간보다 느리게 흐르므로 유의해주시기 바랍니다.");
        m_Text.Add("지금부터 Player님의 휴식을 위해 10초 후에 게임이 자동 종료 되겠습니다.");
        m_TextCnt = 0;

        m_Player = GameObject.FindWithTag("Player");
    }

    private void SetActive(bool active)
    {
        for (int i = 0; i < transform.childCount; ++i)
            transform.GetChild(i).gameObject.SetActive(active);

        m_bActive = active;
    }
}
