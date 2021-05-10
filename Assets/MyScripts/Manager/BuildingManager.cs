using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuildingManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name != "MainScene")
            gameObject.SetActive(false);
    }

    public int GetBuildingCount(string name)
    {
        int count = 0;

        foreach(Transform building in gameObject.GetComponentsInChildren<Transform>())
        {
            if (building.name == name)
                ++count;
        }

        return count;
    }
}
