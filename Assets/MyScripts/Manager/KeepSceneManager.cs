using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeepSceneManager : MonoBehaviour
{
    private void Awake()
    {
        var objs = FindObjectsOfType<KeepSceneManager>();

        if (objs.Length == 1)
            DontDestroyOnLoad(gameObject);
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "TitleScene")
        {
            Cursor.visible = true;
            Destroy(gameObject);
        }
    }
}
