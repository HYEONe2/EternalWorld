using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSceneManager : MonoBehaviour
{
    [SerializeField] public string m_SceneName;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("0"))
            LoadingSceneManager.LoadScene("MainScene");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            LoadingSceneManager.LoadScene(m_SceneName);
    }

    public void StartGame()
    {
        LoadingSceneManager.LoadScene(m_SceneName);
    }

    public void EndGame()
    {
        Application.Quit();
    }
}
