using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    private string m_CurScene;
    private string m_NextScene;

    public enum TYPE { TYPE_PLAYER, TYPE_MONSTER, TYPE_UI, TYPE_END};

    private List<AudioClip> m_BGMList = new List<AudioClip>();
    private static List<AudioClip> m_PlayerList = new List<AudioClip>();
    private static List<AudioClip> m_MonsterList = new List<AudioClip>();
    private static List<AudioClip> m_UIList = new List<AudioClip>();

    private AudioSource m_SoundAudio;
    private static bool m_bEnding;

    private bool m_bBGMMute;
    private float m_BGMVolume;

    private bool m_bEffectMute;
    private static float m_EffectVolume;

    private Sprite[] m_BGMSprite = new Sprite[2];

    public static void SetEnding(bool bEnding) {  m_bEnding = bEnding;  }
    public static bool GetEnding() { return m_bEnding; }

    public void SetBGMVolume(float value) { m_BGMVolume = value; }
    public void SetBGMMute(Button button)
    {
        m_bBGMMute = !m_bBGMMute;

        if (m_bBGMMute)
            button.image.sprite = m_BGMSprite[1];
        else
            button.image.sprite = m_BGMSprite[0];
    }

    public void SetEffectVolume(float value) { m_EffectVolume = value; }
    public void SetEffectMute(Button button)
    {
        m_bEffectMute = !m_bEffectMute;

        if (m_bEffectMute)
            button.image.sprite = m_BGMSprite[1];
        else
            button.image.sprite = m_BGMSprite[0];
    }

    public void ClickEffectSound() { PlayEffectSound(TYPE.TYPE_UI, GetComponent<AudioSource>(), 5); }

    // Start is called before the first frame update
    void Start()
    {
        m_CurScene = SceneManager.GetActiveScene().name;
        m_NextScene = m_CurScene;

        m_SoundAudio = GameObject.FindWithTag("MainCamera").GetComponent<AudioSource>();
        m_bEnding = false;

        m_BGMList.Add(Resources.Load<AudioClip>("Sound/BGM/TutorialBGM"));
        m_BGMList.Add(Resources.Load<AudioClip>("Sound/BGM/MainBGM"));
        m_BGMList.Add(Resources.Load<AudioClip>("Sound/BGM/MazeBGM"));
        m_BGMList.Add(Resources.Load<AudioClip>("Sound/BGM/EndingBGM"));
        m_BGMList.Add(Resources.Load<AudioClip>("Sound/BGM/LoadingBGM"));

        m_SoundAudio.clip = m_BGMList[0];
        m_SoundAudio.loop = true;

        m_BGMVolume = 0.5f;
        m_EffectVolume = 0.5f;

        m_SoundAudio.volume = m_BGMVolume;
        m_bBGMMute = false;
        m_bEffectMute = false;

        m_BGMSprite[0] = Resources.Load<Sprite>("UI/PlayerCanvas/Play");
        m_BGMSprite[1] = Resources.Load<Sprite>("UI/PlayerCanvas/Mute");

        //0
        m_PlayerList.Add(Resources.Load<AudioClip>("Sound/Effect/Player/GainProperty"));
        m_PlayerList.Add(Resources.Load<AudioClip>("Sound/Effect/Player/PlayerAtt"));
        m_PlayerList.Add(Resources.Load<AudioClip>("Sound/Effect/Player/PlayerHit"));
        m_PlayerList.Add(Resources.Load<AudioClip>("Sound/Effect/Player/PlayerDie"));
        m_PlayerList.Add(Resources.Load<AudioClip>("Sound/Effect/Player/PlayerDie"));
        //5
        m_PlayerList.Add(Resources.Load<AudioClip>("Sound/Effect/Player/Respawn"));
        m_PlayerList.Add(Resources.Load<AudioClip>("Sound/Effect/Player/LevelUp"));
        m_PlayerList.Add(Resources.Load<AudioClip>("Sound/Effect/Player/ChangeEquipment"));
        m_PlayerList.Add(Resources.Load<AudioClip>("Sound/Effect/Player/Footstep"));
        //10

        //0
        m_MonsterList.Add(Resources.Load<AudioClip>("Sound/Effect/Monster/RobotHit"));
        m_MonsterList.Add(Resources.Load<AudioClip>("Sound/Effect/Monster/FireMonsterAtt"));
        m_MonsterList.Add(Resources.Load<AudioClip>("Sound/Effect/Monster/FireMonsterHit"));
        m_MonsterList.Add(Resources.Load<AudioClip>("Sound/Effect/Monster/FireMonsterDeath"));
        m_MonsterList.Add(Resources.Load<AudioClip>("Sound/Effect/Monster/GreenMonsterAtt"));
        //5
        m_MonsterList.Add(Resources.Load<AudioClip>("Sound/Effect/Monster/GreenMonsterHit"));
        m_MonsterList.Add(Resources.Load<AudioClip>("Sound/Effect/Monster/GreenMonsterDeath"));
        m_MonsterList.Add(Resources.Load<AudioClip>("Sound/Effect/Monster/WaterMonsterAtt"));
        m_MonsterList.Add(Resources.Load<AudioClip>("Sound/Effect/Monster/WaterMonsterHit"));
        m_MonsterList.Add(Resources.Load<AudioClip>("Sound/Effect/Monster/WaterMonsterDeath"));

        //0
        m_UIList.Add(Resources.Load<AudioClip>("Sound/Effect/UI/Enterkey"));
        m_UIList.Add(Resources.Load<AudioClip>("Sound/Effect/UI/Success"));
        m_UIList.Add(Resources.Load<AudioClip>("Sound/Effect/UI/Build"));
        m_UIList.Add(Resources.Load<AudioClip>("Sound/Effect/UI/Upgrade"));
        m_UIList.Add(Resources.Load<AudioClip>("Sound/Effect/UI/BuySell"));
        //5
        m_UIList.Add(Resources.Load<AudioClip>("Sound/Effect/UI/Click"));
        m_UIList.Add(Resources.Load<AudioClip>("Sound/Effect/UI/FailBuySell"));
    }

    // Update is called once per frame
    void Update()
    {
        if (m_bEnding)
        {
            if (m_SoundAudio.clip == m_BGMList[1])
                m_SoundAudio.Stop();

            if (m_SoundAudio.isPlaying)
                return;

            m_SoundAudio.clip = m_BGMList[3];
            m_SoundAudio.loop = true;
            m_SoundAudio.volume = 0.5f;
            m_SoundAudio.Play();
        }
        else
        {
            UpdateVolume();

            if (!CheckScene())
                return;
            UpdateBGM();
        }
    }

    private void UpdateVolume()
    {
        if (m_bBGMMute)
            m_SoundAudio.Stop();
        else
        {
            if (!m_SoundAudio)
                return;

            if(!m_SoundAudio.isPlaying)
                 m_SoundAudio.Play();
        }

        m_SoundAudio.volume = m_BGMVolume;
    }

    private bool CheckScene()
    {
        m_NextScene = SceneManager.GetActiveScene().name;

        if (m_CurScene == m_NextScene)
            return false;
        else
        {
            m_CurScene = m_NextScene;
            return true;
        }
    }

    private void UpdateBGM()
    {
        switch (m_CurScene)
        {
            case "TutorialScene":
                m_SoundAudio.clip = m_BGMList[0];
                break;
            case "MainScene":
                m_SoundAudio.clip = m_BGMList[1];
                break;
            case "MazeScene":
                m_SoundAudio.clip = m_BGMList[2];
                break;
            case "LoadingScene":
                m_SoundAudio.clip = m_BGMList[4];
                break;
        }

        m_SoundAudio.loop = true;
        m_SoundAudio.volume = m_BGMVolume;
        m_SoundAudio.Play();
    }

    static public void PlayEffectSound(TYPE eType, AudioSource audioSource, int index)
    {
        switch(eType)
        {
            case TYPE.TYPE_PLAYER:
                audioSource.clip = m_PlayerList[index];
                break;
            case TYPE.TYPE_MONSTER:
                audioSource.clip = m_MonsterList[index];
                break;
            case TYPE.TYPE_UI:
                audioSource.clip = m_UIList[index];
                break;
        }

        audioSource.Play();
        audioSource.volume = m_EffectVolume;
    }
}
