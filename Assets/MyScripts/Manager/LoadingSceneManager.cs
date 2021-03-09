using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    public static string curScene;
    public static string nextScene;

    private Image m_ProgressBar;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadScene());
    }

    public static void LoadScene(string sceneName)
    {
        curScene = SceneManager.GetActiveScene().name;
        nextScene = sceneName;

        SceneManager.LoadScene("LoadingScene");
    }

    IEnumerator LoadScene()
    {
        yield return null;

        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
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

                    yield break;
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
