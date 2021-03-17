using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoSceneSwitch : MonoBehaviour
{
    Scene currentScene;
    private void Start()
    {
        currentScene = SceneManager.GetActiveScene();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Scene Switch"))
        {
            if (currentScene.buildIndex == 0)
            {
                SceneManager.LoadScene(1);
            }
            else if (currentScene.buildIndex == 1)
            {
                SceneManager.LoadScene(2);
            }
            else if (currentScene.buildIndex == 2)
            {
                SceneManager.LoadScene(0);
            }
        }
    }
}
