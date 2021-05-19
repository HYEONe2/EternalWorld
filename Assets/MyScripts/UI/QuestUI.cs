using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    // Child Components
    private Image m_AIImage;
    private Text m_QuestText;
    private GameObject m_EnterText;
    private Sprite[] m_Images;
    private AudioSource m_EffectSound;

    private GameObject m_Answer;
    private GameObject m_AnswerOnce;

    private GameObject m_Axe;

    // Other Components
    private UIManager m_UIManager;
    private ObjectManager m_ObjMgrScript;
    private Player m_Player;
    private PlayerProperty m_PlayerProperty;

    // Values
    public enum STATE { TALK, QUEST, ASK, PAUSE, END };
    private STATE m_eState;

    private List<List<string>> m_TalkingText = new List<List<string>>();
    private int m_CheckEvent;
    private int m_TextCnt;

    private bool[] m_bCheckList = new bool[4];
    private bool m_bFinishTask;
    private bool m_bFinishSpawn;

    private int m_WoodCnt;

    public STATE GetState() { return m_eState; }

    // Start is called before the first frame update
    void Start()
    {
        m_AIImage = transform.Find("AIImage").GetComponent<Image>();
        m_QuestText = transform.Find("QuestText").GetComponent<Text>();
        m_EnterText = transform.Find("EnterText").gameObject;
        m_Images = Resources.LoadAll<Sprite>("UI/QuestCanvas");
        m_EffectSound = GetComponent<AudioSource>();

        m_Answer = transform.Find("Answer").gameObject;
        m_Answer.SetActive(false);
        m_AnswerOnce = transform.Find("AnswerOnce").gameObject;
        m_AnswerOnce.SetActive(false);

        m_Axe = GameObject.Find("Axe");
        m_Axe.SetActive(false);

        m_UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        m_ObjMgrScript = GameObject.Find("ObjectManager").GetComponent<ObjectManager>();
        m_Player = GameObject.Find("Player").GetComponent<Player>();
        m_PlayerProperty = GameObject.Find("Player").GetComponent<PlayerProperty>();

        m_eState = STATE.TALK;

        InitializeTalkingText();
        m_CheckEvent = 0;
        m_TextCnt = 0;

        m_bFinishTask = false;
        m_bFinishSpawn = false;

        m_WoodCnt = 0;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateQuestText();
    }

    private void OnDestroy()
    {
        for (int i = 0; i < m_TalkingText.Count; ++i)
            m_TalkingText[i].Clear();

        m_TalkingText.Clear();
    }

    /*
    영토확장 ->     
    나무 돌 캠(한번만 생성됨) ->
    재화수급 -> 
    얻은 재화로 건물 건설(나무,돌같은게 한번만 생성되니깐,,건물로 승화) ->
    상점에서 사고팜(플레이어 경험치오름 + 희귀보석 얻음) + 던전 파밍-> 
    플레이어 레벨오르면 영토확장
    */
    private void InitializeTalkingText()
    {
        List<string> FirstTalking = new List<string>();
        List<string> SecondTalking = new List<string>();
        List<string> ThirdTalking = new List<string>();
        List<string> ForthTalking = new List<string>();
        List<string> FifthTalking = new List<string>();
        List<string> SixthTalking = new List<string>();
        List<string> SeventhTalking = new List<string>();
        List<string> EighthTalking = new List<string>();
        List<string> NinethTalking = new List<string>();
        List<string> TenthTalking = new List<string>();
        List<string> EleventhTalking = new List<string>();
        List<string> TwelfthTalking = new List<string>();
        List<string> ThirteenthTalking = new List<string>();
        List<string> FourteenthTalking = new List<string>();
        List<string> FifteenthTalking = new List<string>();
        List<string> SixteenthTalking = new List<string>();
        List<string> SeventeenthTalking = new List<string>();
        List<string> EighteenthTalking = new List<string>();
        List<string> NineteenthTalking = new List<string>();
        List<string> TwelvethTalking = new List<string>();

        //1 ASK
        FirstTalking.Add("어서오세요. Player님.");
        FirstTalking.Add("저희 VR 게임인 Eternal World를 이용해주셔서 감사합니다.");
        FirstTalking.Add("저희 Eternal World에서는 Player님만의 마을을 만들 수 있도록 가상 월드를 제공합니다.");
        FirstTalking.Add("이곳은 마을로 입성하기 전 기능 체험을 위한 가상 튜토리얼 공간입니다.");
        FirstTalking.Add("Player님만의 세상을 만들기에 앞서 Player님의 게임 이용 여부를 확인하겠습니다.");
        FirstTalking.Add("...");
        FirstTalking.Add("Player님의 게임 이용 여부 확인 결과 없음으로 판명되었습니다.");
        FirstTalking.Add("이 게임을 위한 튜토리얼을 진행하겠습니까?");
        m_TalkingText.Add(FirstTalking);

        //2 WSAD 퀘스트
        SecondTalking.Add("좋습니다. 그럼 튜토리얼을 시작하겠습니다.");
        SecondTalking.Add("먼저 기본 조작법 설명입니다.");
        SecondTalking.Add("WASD 키를 이용하여 몸을 움직여 보십시오.");
        m_TalkingText.Add(SecondTalking);

        //3 Mouse 시야 테스트
        ThirdTalking.Add("잘하셨습니다.");
        ThirdTalking.Add("이제 마우스를 움직여 시야를 조절해 보십시오.");
        m_TalkingText.Add(ThirdTalking);

        //4
        ForthTalking.Add("잘하셨습니다.");
        ForthTalking.Add("Shift 키를 눌러 달려보십시오.");
        m_TalkingText.Add(ForthTalking);

        //5
        FifthTalking.Add("잘하셨습니다.");
        FifthTalking.Add("Space 키를 눌러 점프하십시오.");
        m_TalkingText.Add(FifthTalking);

        //6 영토 설명 Queset
        SixthTalking.Add("좋습니다. 기본 이동 기술을 모두 익혔습니다.");
        SixthTalking.Add("Player님께 기본 제공품인 도끼를 드리기 앞서 영토 설명을 해드리겠습니다.");
        SixthTalking.Add("먼저 다음 장소인 눈 앞의 통로로 이동해보십시오.");
        m_TalkingText.Add(SixthTalking);

        //7 재화획득 퀘스트
        SeventhTalking.Add("Player는 Level에 따라 영토가 한정되어 있기 때문에 투명 장벽으로 일정 범위 이외로 나갈 수 없도록 설계되어 있습니다.");
        SeventhTalking.Add("따라서 Player는 나의 마을 영토를 넓히기 위해 레벨을 올려야 합니다.");
        SeventhTalking.Add("레벨을 올리기 위한 경험치를 재화 획득, 건물 건설, 상점 이용, 던전 파밍을 통해 얻을 수 있습니다.");
        SeventhTalking.Add("먼저 재화 획득에 대해 알려드리도록 하겠습니다.");
        SeventhTalking.Add("재화 획득의 방법에는 크게 두 가지가 있습니다.");
        SeventhTalking.Add("첫 번째로는 내 영토 내에 있는 오브젝트에서 획득하는 방법과");
        SeventhTalking.Add("두 번째로는 건물 건설을 통한 재화 방법이 있습니다.");
        SeventhTalking.Add("첫 번째 방법 튜토리얼을 위한 도끼를 제공해드리겠습니다.");
        SeventhTalking.Add("F키를 통해 집어주십시오.");
        m_TalkingText.Add(SeventhTalking);

        // 8 
        EighthTalking.Add("잘하셨습니다.");
        EighthTalking.Add("이제 1번 키로 도끼를 집어 넣어주십시오.");
        m_TalkingText.Add(EighthTalking);

        //9
        NinethTalking.Add("잘하셨습니다.");
        NinethTalking.Add("다시 1번 키로 도끼를 꺼내주십시오.");
        m_TalkingText.Add(NinethTalking);

        //10
        TenthTalking.Add("잘하셨습니다.");
        TenthTalking.Add("이제 제가 생성해드릴 나무와 돌을 1개씩 채집해주십시오.");
        TenthTalking.Add("F 버튼을 이용하여 재화를 얻을 수 있습니다.");
        m_TalkingText.Add(TenthTalking);

        EleventhTalking.Add("잘하셨습니다.");
        EleventhTalking.Add("지금 한 작업은 재화 획득의 첫 번째 방법인 나의 영토 내의 오브젝트로부터 재화를 수집하는 방법입니다.");
        EleventhTalking.Add("두 번째 방법인 건물 건설을 통한 재화 획득을 해보도록 합시다.");
        EleventhTalking.Add("I 버튼을 눌러 스마트폰을 꺼내보십시오.");
        m_TalkingText.Add(EleventhTalking);

        //12
        TwelfthTalking.Add("스마트폰을 켜면 인벤토리, 상점, 건물 짓기, 세팅 어플이 있습니다.");
        TwelfthTalking.Add("먼저 인벤토리에 아까 채집한 나무와 돌의 개수를 확인하여주십시오.");
        m_TalkingText.Add(TwelfthTalking);

        //13
        ThirteenthTalking.Add("좋습니다.");
        ThirteenthTalking.Add("이제 메인 화면으로 돌아간 뒤, 건물 짓기 어플을 이용하여 나무집을 지어보십시오.");
        m_TalkingText.Add(ThirteenthTalking);

        //16 -> 14
        SixteenthTalking.Add("잘하셨습니다.");
        SixteenthTalking.Add("지금까지 경험치를 얻기 위한 재화 습득의 두 가지 방법과 건물 짓기에 대해 알아보았습니다.");
        SixteenthTalking.Add("이제 세 번째 경험치 획득 방법인 상점 이용에 대해 알려드리겠습니다.");
        SixteenthTalking.Add("상점은 판매와 구입이 가능하며 상점 이용을 하다보면 업그레이드 원석을 얻을 수도 있습니다.");
        SixteenthTalking.Add("판매와 구입은 우측 상단에 있는 버튼을 통해 변경할 수 있습니다.");
        SixteenthTalking.Add("그럼 판매와 구입 활동을 통하여 업그레이드 원석을 하나 얻어보십시오.");
        m_TalkingText.Add(SixteenthTalking);

        //14 -> 15
        FourteenthTalking.Add("잘하셨습니다.");
        FourteenthTalking.Add("지어진 건물을 클릭하면 해당 건물에 투명화가 적용되며 지목화가 됩니다.");
        FourteenthTalking.Add("지목된 건물은 어플에 정보가 뜨게 되고 건물 재배치와 업그레이드가 가능하게 됩니다.");
        FourteenthTalking.Add("건물 재배치로 위치를 옮겨보고 원석을 집어 업그레이드까지 진행하여 보십시오.");
        FourteenthTalking.Add("참고로 건물 재배치는 시야 회전을 통한 위치 조절과 마우스 휠을 통해 건물 자체를 회전시킬 수 있습니다.");
        m_TalkingText.Add(FourteenthTalking);

        //15 -> 16
        FifteenthTalking.Add("잘하셨습니다.");
        FifteenthTalking.Add("업그레이드를 진행하면 건물의 생산속도가 올라가게 되거나 생산품의 개수가 올라갑니다.");
        FifteenthTalking.Add("건물에서 생산이 완료된 경우 건물에 반짝이는 이펙트가 뜨게 됩니다.");
        FifteenthTalking.Add("이 때 건물 근처로 가 F 버튼을 누르면 재화를 획득하실 수 있습니다.");
        FifteenthTalking.Add("F 버튼을 눌러 재화를 획득하여 보십시오.");
        m_TalkingText.Add(FifteenthTalking);
               
        //17
        SeventeenthTalking.Add("좋습니다.");
        SeventeenthTalking.Add("이제 경험치를 얻을 수 있는 마지막 방법인 던전 파밍에 대한 안내를 해드리겠습니다..");
        SeventeenthTalking.Add("던전 파밍을 알려드리기 전 Player 능력을 고르는 시간을 갖도록 하겠습니다.");
        SeventeenthTalking.Add("어떤 능력을 습득하시겠습니까?");
        m_TalkingText.Add(SeventeenthTalking);

        //17
        EighteenthTalking.Add("좋습니다.");
        EighteenthTalking.Add("던전 파밍을 위해 다음 영토를 확장해야 하기 때문에 레벨을 임의로 올려드리겠습니다.");
        EighteenthTalking.Add("이제 F 버튼을 통해 무기를 집고 2번 버튼을 눌러 장착해주십시오.");
        EighteenthTalking.Add("마우스 왼 클릭으로 근접 공격을 해보십시오.");
        m_TalkingText.Add(EighteenthTalking);

        //18
        NineteenthTalking.Add("잘하셨습니다.");
        NineteenthTalking.Add("1번은 도끼 버튼이며 2번은 무기 버튼임을 기억해주시기 바랍니다.");
        NineteenthTalking.Add("이제 몬스터 타겟팅에 대해 설명해드리도록 하겠습니다.");
        NineteenthTalking.Add("우클릭을 하면 일정 거리 안 몬스터 중 하나에게 타겟팅이 적용되게 됩니다.");
        NineteenthTalking.Add("타겟팅을 사용하게 되면 플레이어가 바라보는 방향으로 시전되는 스킬 사용이 편리해집니다.");
        NineteenthTalking.Add("우클릭을 통해 타겟팅 후, q 키를 눌러 원거리 스킬을 사용하여 보십시오.");
        m_TalkingText.Add(NineteenthTalking);

        //19
        TwelvethTalking.Add("잘하셨습니다.");
        TwelvethTalking.Add("E키와 R키로 각각 다른 스킬을 사용할 수 있습니다.");
        TwelvethTalking.Add("Player님께서 원하시는 만큼 여러 기능들을 체험하시고 체험이 끝나면 이 튜토리얼 방 끝에 있는 문으로 와주십시오.");
        TwelvethTalking.Add("Player님만의 세계를 구축할 수 있도록 준비해놓도록 하겠습니다.");
        m_TalkingText.Add(TwelvethTalking);
    }

    private void UpdateQuestText()
    {
        // 마지막 인덱스에 도달한 경우 -> 새로운 이벤트의 시작
        if (m_TalkingText[m_CheckEvent].Count - 1 <= m_TextCnt)
        {
            if (m_TalkingText.Count - 1 <= m_CheckEvent)
            {
                // Quest UI 종료
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    transform.parent.gameObject.SetActive(false);
                    return;
                }
            }
            else
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
                {
                    SoundManager.PlayEffectSound(SoundManager.TYPE.TYPE_UI, m_EffectSound, 0);
                    ++m_TextCnt;
                }
            }
        }

        // 계속 false이다가 버튼 클릭/퀘스트 클리어하면 true
        // 초기화되면 false로 다시 바뀜
        MainUpdate();
    }

    private void BeforeUpdate()
    {
        // 새로운 이벤트 시작 전, 초기화
        switch(m_CheckEvent + 1)
        {
            // ASK
            case 1:
            case 17:
                {
                    m_bFinishSpawn = false;
                    m_bFinishTask = false;
                    m_CheckEvent += 1;
                    m_TextCnt = 0;
                }
                break;
            // Quest
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
            case 10:
            case 11:
            case 12:
            case 13:
            case 14:
            case 15:
            case 16:
            case 18:
            case 19:
                {
                    if (!m_bFinishTask)
                        return;

                    // Quest가 끝나면 다음 이벤트로 초기화
                    m_bFinishSpawn = false;
                    m_bFinishTask = false;
                    m_CheckEvent += 1;
                    m_TextCnt = 0;
                }
                break;
            // Talk
            default:
                {
                    m_bFinishSpawn = false;
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
        //Debug.Log(m_CheckEvent+": "+m_eState);
        SpawnEvent();

        switch(m_eState)
        {
            case STATE.TALK:
                ResetCheckList();
                if (!m_EnterText.activeSelf)
                    m_EnterText.SetActive(true);
                SetQuestText(m_TalkingText[m_CheckEvent][m_TextCnt]);
                break;
            case STATE.QUEST:
                if(m_EnterText.activeSelf)
                  m_EnterText.SetActive(false);
                UpdateQuest();
                break;
            case STATE.ASK:
                if (m_EnterText.activeSelf)
                    m_EnterText.SetActive(false);
                UpdateAsk();
                break;
        }
    }

    private void CheckState()
    {
        switch (m_CheckEvent)
        {
            case 1:
            case 17:
                if (!m_bFinishTask)
                    m_eState = STATE.ASK;
                else
                {
                    if (m_Answer.activeSelf)
                    {
                        Cursor.visible = false;
                        m_Answer.SetActive(false);
                    }
                    else if(m_AnswerOnce.activeSelf)
                    {
                        Cursor.visible = false;
                        m_AnswerOnce.SetActive(false);
                    }
                    m_eState = STATE.TALK;
                }
                break;
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
            case 10:
            case 11:
            case 12:
            case 13:
            case 14:
            case 15:
            case 16:
            case 18:
            case 19:
                {
                    if (!m_bFinishTask)
                        m_eState = STATE.QUEST;
                    else
                        m_eState = STATE.TALK;
                }
                break;
        }
    }

    private void UpdateAsk()
    {
        switch (m_CheckEvent)
        {
            case 1:
                {
                    if (!m_Answer.activeSelf)
                    {
                        Cursor.visible = true;
                        m_Answer.SetActive(true);
                    }

                    if (m_bCheckList[0])
                        m_bFinishTask = true;
                    else if (m_bCheckList[1])
                    {
                        Cursor.visible = false;
                        m_UIManager.SetPhoneCanvasActive(true);
                        m_UIManager.SetQuestCanves(null);
                        Destroy(gameObject);
                        // 씬 전환
                    }
                }
                break;
            case 17:
                {
                    if (!m_AnswerOnce.activeSelf)
                    {
                        Cursor.visible = true;
                        m_AnswerOnce.SetActive(true);
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
                            {
                                SoundManager.PlayEffectSound(SoundManager.TYPE.TYPE_UI, m_EffectSound, 1);
                                m_bFinishTask = true;
                            }
                        }
                    }
                }
                break;
            case 3:
                {
                    Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                    if (mouseDelta.magnitude != 0)
                    {
                        SoundManager.PlayEffectSound(SoundManager.TYPE.TYPE_UI, m_EffectSound, 1);
                        m_bFinishTask = true;
                    }
                }
                break;
            case 4:
                {
                    if(Input.GetKey(KeyCode.LeftShift))
                    {
                        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)
                            || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                        {
                            SoundManager.PlayEffectSound(SoundManager.TYPE.TYPE_UI, m_EffectSound, 1);
                            m_bFinishTask = true;
                        }
                    }
                }
                break;
            case 5:
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        SoundManager.PlayEffectSound(SoundManager.TYPE.TYPE_UI, m_EffectSound, 1);
                        m_bFinishTask = true;
                    }
                }
                break;
            case 6:
                {
                    if (m_ObjMgrScript.GetCloseEnough())
                    {
                        SoundManager.PlayEffectSound(SoundManager.TYPE.TYPE_UI, m_EffectSound, 1);
                        m_bFinishTask = true;
                    }
                }
                break;
            case 7:
                {
                    if (m_Player.GetActive("Equipment"))
                    {
                        SoundManager.PlayEffectSound(SoundManager.TYPE.TYPE_UI, m_EffectSound, 1);
                        m_bFinishTask = true;
                    }
                }
                break;
            case 8:
                {
                    if (Input.GetKeyDown("1"))
                    {
                        if (!m_Player.GetActive("Equipment"))
                        {
                            SoundManager.PlayEffectSound(SoundManager.TYPE.TYPE_UI, m_EffectSound, 1);
                            m_bFinishTask = true;
                        }
                    }
                }
                break;
            case 9:
                {
                    if (Input.GetKeyDown("1"))
                    {
                        if (m_Player.GetActive("Equipment"))
                        {
                            SoundManager.PlayEffectSound(SoundManager.TYPE.TYPE_UI, m_EffectSound, 1);
                            m_bFinishTask = true;
                        }
                    }
                }
                break;
            case 10:
                {
                    if (m_PlayerProperty.GetPropertyAmount(PlayerProperty.OBJTYPE.OBJ_WOOD) >= 1
                        && m_PlayerProperty.GetPropertyAmount(PlayerProperty.OBJTYPE.OBJ_STONE) >= 1)
                    {
                        SoundManager.PlayEffectSound(SoundManager.TYPE.TYPE_UI, m_EffectSound, 1);
                        m_bFinishTask = true;
                    }
                }
                break;
            case 11:
                {
                    if (!m_UIManager.GetActive("PhoneCanvas"))
                        return;

                    if (Input.GetKeyDown(KeyCode.I))
                    {
                        SoundManager.PlayEffectSound(SoundManager.TYPE.TYPE_UI, m_EffectSound, 1);
                        m_bFinishTask = true;
                    }
                }
                break;
            case 12:
                {
                    if (m_UIManager.GetActive("InventoryUI"))
                    {
                        SoundManager.PlayEffectSound(SoundManager.TYPE.TYPE_UI, m_EffectSound, 1);
                        m_bFinishTask = true;
                    }
                }
                break;
            case 13:
                {
                    GameObject WoodHouse = GameObject.Find("WoodHouse(Clone)");
                    if(WoodHouse)
                    {
                        if (WoodHouse.GetComponent<Building>().GetBuild() == Building.BUILD.BUILT)
                        {
                            SoundManager.PlayEffectSound(SoundManager.TYPE.TYPE_UI, m_EffectSound, 1);
                            m_bFinishTask = true;
                        }
                    }
                }
                break;
            case 14:
                {
                    if (m_UIManager.GetEarnGem())
                    {
                        SoundManager.PlayEffectSound(SoundManager.TYPE.TYPE_UI, m_EffectSound, 1);
                        m_bFinishTask = true;
                    }
                }
                break;
            case 15:
                {
                    // 건물 재배치 & 업그레이드
                    if (m_UIManager.GetRebuildUpgrade())
                    {
                        SoundManager.PlayEffectSound(SoundManager.TYPE.TYPE_UI, m_EffectSound, 1);
                        m_bFinishTask = true;
                    }

                    m_WoodCnt = m_PlayerProperty.GetPropertyAmount(PlayerProperty.OBJTYPE.OBJ_WOOD);
                }
                break;
            case 16:
                {
                    // 재화 획득
                    if (m_WoodCnt < m_PlayerProperty.GetPropertyAmount(PlayerProperty.OBJTYPE.OBJ_WOOD))
                    {
                        SoundManager.PlayEffectSound(SoundManager.TYPE.TYPE_UI, m_EffectSound, 1);
                        m_bFinishTask = true;
                    }
                }
                break;
            case 18:
                {
                    if (!m_Player.GetActive("Weapon"))
                        return;

                    if (Input.GetMouseButtonDown(0))
                    {
                        SoundManager.PlayEffectSound(SoundManager.TYPE.TYPE_UI, m_EffectSound, 1);
                        m_bFinishTask = true;
                    }
                }
                break;
            case 19:
                {
                    if (!m_Player.GetActive("Weapon"))
                        return;

                    if (Input.GetMouseButton(1))
                    {
                        if (Input.GetKeyDown(KeyCode.Q))
                        {
                            SoundManager.PlayEffectSound(SoundManager.TYPE.TYPE_UI, m_EffectSound, 1);
                            m_bFinishTask = true;
                        }
                    }
                }
                break;
        }
    }

    private void SpawnEvent()
    {
        if (m_bFinishSpawn)
            return;

        if (m_CheckEvent == 6 && m_TextCnt == 8)
            m_Axe.SetActive(true);
        else if (m_CheckEvent == 10 && m_TextCnt == 0)
            m_ObjMgrScript.RespawnObjects(3, 3);
        else if (m_CheckEvent == 17 && m_TextCnt == 2)
            m_ObjMgrScript.DestroyLevelBoundary();
        else
            return;

        m_bFinishSpawn = true;
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
        SoundManager.PlayEffectSound(SoundManager.TYPE.TYPE_UI, m_EffectSound, 0);
        m_bCheckList[0] = true;
    }

    public void ClickNoButton()
    {
        Cursor.visible = false;
        m_Answer.SetActive(false);

        SoundManager.PlayEffectSound(SoundManager.TYPE.TYPE_UI, m_EffectSound, 0);
        m_bCheckList[1] = true;
    }

    public void ClickFireButton()
    {
        Cursor.visible = false;
        m_AnswerOnce.SetActive(false);

        GameObject.Find("Player").GetComponent<Player>().SetAbility(ObjectManager.ABILITY.ABIL_FIRE);
        SoundManager.PlayEffectSound(SoundManager.TYPE.TYPE_UI, m_EffectSound, 0);
        m_bFinishTask = true;
    }

    public void ClickWaterButton()
    {
        Cursor.visible = false;
        m_AnswerOnce.SetActive(false);

        GameObject.Find("Player").GetComponent<Player>().SetAbility(ObjectManager.ABILITY.ABIL_WATER);
        SoundManager.PlayEffectSound(SoundManager.TYPE.TYPE_UI, m_EffectSound, 0);
        m_bFinishTask = true;
    }

    public void ClickGrassButton()
    {
        Cursor.visible = false;
        m_AnswerOnce.SetActive(false);

        GameObject.Find("Player").GetComponent<Player>().SetAbility(ObjectManager.ABILITY.ABIL_GRASS);
        SoundManager.PlayEffectSound(SoundManager.TYPE.TYPE_UI, m_EffectSound, 0);
        m_bFinishTask = true;
    }
}
