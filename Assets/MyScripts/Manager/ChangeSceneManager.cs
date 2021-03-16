using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("9"))
            LoadingSceneManager.LoadScene("TutorialScene");

        if (Input.GetKeyDown("0"))
            LoadingSceneManager.LoadScene("TitleScene");
    }
}
