using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    // Child Components
    private Image m_AIImage;
    private Text m_QuestText;
    private Sprite[] m_Images;

    private GameObject m_Answer;
    private GameObject m_AnswerOnce;

    // Values
    public enum STATE { TALK, QUEST, ASK, PAUSE, END };
    private STATE m_eState;

    private List<List<string>> m_TalkingText = new List<List<string>>();
    private int m_CheckEvent;
    private int m_TextCnt;

    private bool[] m_bCheckList = new bool[4];
    private bool m_bFinishTask;

    public STATE GetState() { return m_eState; }

    // Start is called before the first frame update
    void Start()
    {
        m_AIImage = transform.Find("AIImage").GetComponent<Image>();
        m_QuestText = transform.Find("QuestText").GetComponent<Text>();
        m_Images = Resources.LoadAll<Sprite>("UI/QuestCanvas");

        m_Answer = transform.Find("Answer").gameObject;
        m_Answer.SetActive(false);
        m_AnswerOnce = transform.Find("AnswerOnce").gameObject;
        m_AnswerOnce.SetActive(false);

        m_eState = STATE.TALK;

        InitializeTalkingText();
        m_CheckEvent = 0;
        m_TextCnt = 0;

        m_bFinishTask = false;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAIImages();
        UpdateQuestText();
    }

    private void OnDestroy()
    {
        for (int i = 0; i < m_TalkingText.Count; ++i)
            m_TalkingText[i].Clear();

        m_TalkingText.Clear();
    }

    private void InitializeTalkingText()
    {
        List<string> FirstTalking = new List<string>();
        List<string> SecondTalking = new List<string>();
        List<string> ThirdTalking = new List<string>();
        List<string> ForthTalking = new List<string>();

        // Talking
        FirstTalking.Add("어서오세요. Player님.");
        FirstTalking.Add("저희 VR 게임을 이용해주셔서 감사합니다.");
        FirstTalking.Add("Player님의 게임 이용 여부를 확인하겠습니다.");
        FirstTalking.Add("...");
        FirstTalking.Add("Player님의 게임 이용 여부 확인 결과 없음으로 판명되었습니다.");
        FirstTalking.Add("이 게임을 위한 튜토리얼을 진행하겠습니까?");
        //FirstTalking.Add("먼저 게임 적응을 위한 튜토리얼을 진행하겠습니다.");
        m_TalkingText.Add(FirstTalking);

        // Asking
        SecondTalking.Add("그럼 튜토리얼 시작하도록 하겠습니다.");
        SecondTalking.Add("먼저 WSAD를 이용하여 몸을 움직여 보십시오.");
        m_TalkingText.Add(SecondTalking);

        // Talking
        ThirdTalking.Add("잘하셨습니다.");
        ThirdTalking.Add("이제 마우스를 움직여 시야를 조절해 보십시오.");
        m_TalkingText.Add(ThirdTalking);

        ForthTalking.Add("잘하셨습니다.");
        m_TalkingText.Add(ForthTalking);

        // Giving Quest

    }

    private void UpdateAIImages()
    {
        //for (int i = 0; i < m_Images.Length; ++i)
        //{
        //    Debug.Log(i);
        //    m_AIImage.sprite = m_Images[i];
        //}
    }

    private void UpdateQuestText()
    {
        //    // m_Talking[0][5]의 5 <= m_TextCnt
        //    // m_Talking[1][1]의 1 <= m_TextCnt
        //    // m_Talking[2][1]의 1 <= m_TextCnt
        // 마지막 인덱스에 도달한 경우 -> 새로운 이벤트의 시작
        if (m_TalkingText[m_CheckEvent].Count - 1 <= m_TextCnt)
        {
            if (m_TalkingText.Count - 1 <= m_CheckEvent + 1)
            {
                // Quest UI 종료
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    transform.parent.gameObject.SetActive(false);
                    return;
                }
            }

            BeforeUpdate();
        }
        // 마지막 인덱스가 아닌 경우
        else
        {
            // Enter키 누르면
            if (Input.GetKeyDown(KeyCode.Return))
            {
                // 인덱스가 하나씩 늘어남
                if (m_eState == STATE.TALK || m_eState == STATE.PAUSE)
                    ++m_TextCnt;
            }
        }

        // 계속 false이다가 버튼 클릭/퀘스트 클리어하면 true
        // 초기화되면 false로 다시 바뀜
        //Debug.Log("1: "+m_bFinishTask);
        MainUpdate();
        //Debug.Log("2: " + m_bFinishTask);
        //AfterUpdate();

        //Debug.Log(m_eState);
    }

    private void BeforeUpdate()
    {
        // 새로운 이벤트 시작 전, 초기화
        switch(m_CheckEvent)
        {
            // ASK
            case 0:
                {
                    m_Answer.SetActive(false);

                    m_bFinishTask = false;
                    m_CheckEvent += 1;
                    m_TextCnt = 0;
                }
                break;
            // Quest
            case 1:
            case 2:
                {
                    if (!m_bFinishTask)
                        return;

                    // Quest가 끝나면 다음 이벤트로 초기화
                    m_bFinishTask = false;
                    m_CheckEvent += 1;
                    m_TextCnt = 0;
                }
                break;
        }
    }

    private void MainUpdate()
    {
        CheckState();

        switch(m_eState)
        {
            case STATE.TALK:
                ResetCheckList();
                SetQuestText(m_TalkingText[m_CheckEvent][m_TextCnt]);
                break;
            case STATE.QUEST:
                UpdateQuest();
                break;
            case STATE.ASK:
                UpdateAsk();
                break;
        }
    }

    private void CheckState()
    {
        switch (m_CheckEvent)
        {
            case 1:
                if (!m_bFinishTask)
                    m_eState = STATE.ASK;
                else
                {
                    Cursor.visible = false;
                    m_Answer.SetActive(false);
                    m_eState = STATE.TALK;
                }
                break;
            case 2:
            case 3:
                if (!m_bFinishTask)
                    m_eState = STATE.QUEST;
                else
                    m_eState = STATE.TALK;
                break;
        }
    }

    private void UpdateAsk()
    {
        switch (m_CheckEvent)
        {
            case 1:
                {
                    Cursor.visible = true;
                    m_Answer.SetActive(true);

                    if (m_bCheckList[0])
                        m_bFinishTask = true;
                    else if (m_bCheckList[1])
                    {
                        gameObject.SetActive(false);
                        // 씬 전환
                    }
                }
                break;
        }
    }

    private void UpdateQuest()
    {
        switch (m_CheckEvent)
        {
            case 2:
                {
                    if (Input.GetKeyDown(KeyCode.W))
                        m_bCheckList[0] = true;
                    if (Input.GetKeyDown(KeyCode.A))
                        m_bCheckList[1] = true;
                    if (Input.GetKeyDown(KeyCode.S))
                        m_bCheckList[2] = true;
                    if (Input.GetKeyDown(KeyCode.D))
                        m_bCheckList[3] = true;

                    for (int i = 0; i < 4; ++i)
                    {
                        if (!m_bCheckList[i])
                            break;
                        else
                        {
                            if (i == 3)
                                m_bFinishTask = true;
                        }
                    }
                }
                break;
            case 3:
                {
                    Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                    if (mouseDelta.magnitude != 0)
                        m_bFinishTask = true;
                }
                break;
        }
    }

    private void SetQuestText(string text)
    {
        m_QuestText.text = text;
    }

    private void ResetCheckList()
    {
        for (int i = 0; i < m_bCheckList.Length; ++i)
            m_bCheckList[i] = false;
    }

    public void ClickYesButton()
    {
        m_bCheckList[0] = true;
    }

    public void ClickNoButton()
    {
        Cursor.visible = false;
        m_Answer.SetActive(false);

        m_bCheckList[1] = true;
    }

    public void ClickFireButton()
    {
        Cursor.visible = false;
        m_AnswerOnce.SetActive(false);

        GameObject.Find("Player").GetComponent<Player>().SetAbility(Player.ABILITY.ABIL_FIRE);
    }

    public void ClickWaterButton()
    {
        Cursor.visible = false;
        m_AnswerOnce.SetActive(false);

        GameObject.Find("Player").GetComponent<Player>().SetAbility(Player.ABILITY.ABIL_FIRE);
    }

    public void ClickGrassButton()
    {
        Cursor.visible = false;
        m_AnswerOnce.SetActive(false);

        GameObject.Find("Player").GetComponent<Player>().SetAbility(Player.ABILITY.ABIL_FIRE);
    }
}
