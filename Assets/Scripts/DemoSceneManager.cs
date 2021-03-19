using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoSceneManager : MonoBehaviour
{
    [Header("Make sure to put backslash at the end")]
    public string saveLocation = "C:\\Users\\janca\\Documents\\HKU\\Jaar 4\\Afstudeer\\Screenshots\\";
    public int resolutionMultiplier = 2;

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
            if (currentScene.buildIndex + 1 >= SceneManager.sceneCountInBuildSettings) {
                SceneManager.LoadScene(0);
            } else
            {
                SceneManager.LoadScene(currentScene.buildIndex + 1);
            }
            
        } else if (Input.GetButtonDown("Reset"))
        {
            print("reset scene");

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        } else if (Input.GetKeyDown("c"))
        {
            ScreenCapture.CaptureScreenshot(saveLocation + System.DateTime.Now.ToString("yyyy-MM-dd").Replace("-", ".") + "_" + System.DateTime.Now.ToString("HH-mm-ss") + "_" + currentScene.name + "_x" + resolutionMultiplier + ".png", resolutionMultiplier);
        }
    }
}
