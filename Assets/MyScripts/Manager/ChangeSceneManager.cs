using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSceneManager : MonoBehaviour
{
    [SerializeField] public string m_SceneName;

    private bool m_bClick = false;
    private bool m_bSoundPlay = false;
    private bool m_bStart = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (m_bClick)
        {
            AudioSource EffectSound = GameObject.Find("SoundManager").GetComponent<AudioSource>();

            if (!m_bSoundPlay)
            {
                EffectSound.Play();
                m_bSoundPlay = true;
            }
            else
            {
                if (EffectSound.time >= 0.5f)
                {
                    if(m_bStart)
                        LoadingSceneManager.LoadScene(m_SceneName);
                    else
                        Application.Quit();
                }
            }

            return;
        }

        if (Input.GetKeyDown("0"))
            LoadingSceneManager.LoadScene(m_SceneName);
        if (Input.GetKeyDown("9"))
            LoadingSceneManager.LoadScene("TutorialScene");
        if (Input.GetKeyDown("8"))
            LoadingSceneManager.LoadScene("MazeScene");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().ResetCoolTime();
            if (name == "ExitTrigger")
                other.GetComponent<PlayerProperty>().AddClearDungeon();

            LoadingSceneManager.LoadScene(m_SceneName);
        }
    }

    public void StartGame()
    {
        m_bClick = true;
        m_bStart = true;
    }

    public void EndGame()
    {
        m_bClick = true;
    }
}
