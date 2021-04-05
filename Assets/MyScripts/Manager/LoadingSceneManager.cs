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

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadScene());
    }

    public static void LoadScene(string sceneName)
    {
        if (m_CurScene == sceneName)
            return;

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

                    if (m_NextScene == "MainScene")
                    {
                        UIManager uimanager = GameObject.Find("UIManager").GetComponent<UIManager>();
                        if (uimanager)
                        {
                            uimanager.LoadingSetting(true);
                            uimanager.ResetSetting();
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
                        UIManager.GetComponent<UIManager>().LoadingSetting(false);
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
        }
    }
}
