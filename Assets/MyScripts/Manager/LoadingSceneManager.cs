using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    public static string m_CurScene;
    public static string m_NextScene;

    private Image m_ProgressBar;

    private GameObject[] m_Background = new GameObject[4];

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadScene());

        m_Background[0] = GameObject.Find("TutorialImage");
        m_Background[1] = GameObject.Find("MainImage");
        m_Background[2] = GameObject.Find("MazeImage");
        m_Background[3] = GameObject.Find("DefaultImage");
    }

    public static void LoadScene(string sceneName)
    {
        //if (m_CurScene == sceneName)
        //    return;

        m_CurScene = SceneManager.GetActiveScene().name;
        m_NextScene = sceneName;

        SceneManager.LoadScene("LoadingScene");
    }

    IEnumerator LoadScene()
    {
        yield return null;

        AsyncOperation op = SceneManager.LoadSceneAsync(m_NextScene);
        op.allowSceneActivation = false;

        float timer = 0f;
        while (!op.isDone)
        {
            yield return null;

            timer += Time.deltaTime;

            if (op.progress < 0.9f)
            {
                m_ProgressBar.fillAmount = Mathf.Lerp(m_ProgressBar.fillAmount, op.progress, timer);

                if (m_ProgressBar.fillAmount >= op.progress)
                    timer = 0f;
            }
            else
            {
                m_ProgressBar.fillAmount = Mathf.Lerp(m_ProgressBar.fillAmount, 1f, timer);

                if (m_ProgressBar.fillAmount == 1f)
                {
                    // Loading End
                    op.allowSceneActivation = true;

                    if (m_NextScene == "MainScene"
                        || m_NextScene == "MazeScene")
                    {
                        UIManager UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
                        if (UIManager)
                        {
                            UIManager.LoadingSetting(true);
                            UIManager.ResetSetting();
                            UIManager.SetMainQuestUIActive(true);
                        }
                    }

                    yield break;
                }
                else
                {
                    // Loading
                    if (m_NextScene == "TutorialScene")
                        yield return null;

                    GameObject UIManager = GameObject.Find("UIManager");
                    if (UIManager)
                    {
                        UIManager tempManager = UIManager.GetComponent<UIManager>();

                        tempManager.LoadingSetting(false);
                        tempManager.SetMainQuestUIActive(false);
                    }

                    GameObject Player = GameObject.Find("Player");
                    if (Player)
                    {
                        if (m_NextScene == "MainScene")
                        {
                            Player.transform.position = new Vector3(0, 5f, 0);
                            Player.transform.rotation = Quaternion.Euler(Vector3.one);
                            Player.GetComponent<PlayerProperty>().ResetStr();
                        }
                        else if (m_NextScene == "MazeScene")
                        {
                            Player.transform.position = new Vector3(7f, 0, 10f);
                            Player.transform.rotation = Quaternion.Euler(Vector3.one);
                        }

                        SoundManager.PlayEffectSound(SoundManager.TYPE.TYPE_PLAYER, Player.GetComponent<AudioSource>(), 5);
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "LoadingScene")
        {
            if (!m_ProgressBar)
                m_ProgressBar = GameObject.Find("ProgressBar").GetComponent<Image>();

            if(m_NextScene == "TutorialScene")
            {
                m_Background[0].SetActive(true);
                m_Background[1].SetActive(false);
                m_Background[2].SetActive(false);
                m_Background[3].SetActive(false);
            }
            else if(m_NextScene == "MainScene")
            {
                m_Background[0].SetActive(false);
                m_Background[1].SetActive(true);
                m_Background[2].SetActive(false);
                m_Background[3].SetActive(false);
            }
            else if(m_NextScene == "MazeScene")
            {
                m_Background[0].SetActive(false);
                m_Background[1].SetActive(false);
                m_Background[2].SetActive(true);
                m_Background[3].SetActive(false);
            }
            else
            {
                m_Background[0].SetActive(false);
                m_Background[1].SetActive(false);
                m_Background[2].SetActive(false);
                m_Background[3].SetActive(true);
            }
        }
    }
}
