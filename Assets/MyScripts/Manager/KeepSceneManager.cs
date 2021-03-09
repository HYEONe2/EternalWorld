using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
