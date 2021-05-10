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

    private List<AudioClip> m_SoundList = new List<AudioClip>();
    private AudioSource m_SoundAudio;
    private bool m_bMute;
    private float m_BGMVolume;

    private Sprite[] m_BGMSprite = new Sprite[2];

    public void SetBGMVolume(float value) { m_BGMVolume = value; }
    public void SetBGMMute(Button button)
    {
        m_bMute = !m_bMute;

        if (m_bMute)
            button.image.sprite = m_BGMSprite[1];
        else
            button.image.sprite = m_BGMSprite[0];
    }

    public void SetEndingBGM()
    {
        m_SoundAudio.clip = m_SoundList[3];
        m_SoundAudio.loop = true;
        m_SoundAudio.volume = 0.5f;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_CurScene = SceneManager.GetActiveScene().name;
        m_NextScene = m_CurScene;

        m_SoundAudio = GameObject.FindWithTag("MainCamera").GetComponent<AudioSource>();

        m_SoundList.Add(Resources.Load<AudioClip>("Sound/BGM/TutorialBGM"));
        m_SoundList.Add(Resources.Load<AudioClip>("Sound/BGM/MainBGM"));
        m_SoundList.Add(Resources.Load<AudioClip>("Sound/BGM/MazeBGM"));
        m_SoundList.Add(Resources.Load<AudioClip>("Sound/BGM/EndingBGM"));

        m_SoundAudio.clip = m_SoundList[0];
        m_SoundAudio.loop = true;

        m_BGMVolume = 0.5f;
        m_SoundAudio.volume = m_BGMVolume;
        m_bMute = false;

        m_BGMSprite[0] = Resources.Load<Sprite>("UI/PlayerCanvas/Play");
        m_BGMSprite[1] = Resources.Load<Sprite>("UI/PlayerCanvas/Mute");
    }

    // Update is called once per frame
    void Update()
    {
        UpdateVolume();

        if (!CheckScene())
            return;
        UpdateBGM();
    }

    private void UpdateVolume()
    {
        if (m_bMute)
            m_SoundAudio.Stop();
        else
        {
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
                m_SoundAudio.clip = m_SoundList[0];
                break;
            case "MainScene":
                m_SoundAudio.clip = m_SoundList[1];
                break;
            case "MazeScene":
                m_SoundAudio.clip = m_SoundList[2];
                break;
            case "EndingScene":
                m_SoundAudio.clip = m_SoundList[3];
                break;
            default:
                m_SoundAudio.clip = null;
                return;
        }

        m_SoundAudio.loop = true;
        m_SoundAudio.volume = m_BGMVolume;
        m_SoundAudio.Play();
    }
}
