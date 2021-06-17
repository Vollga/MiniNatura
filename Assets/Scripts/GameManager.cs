using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    //Public Reference
    public static GameManager gameManager;
    

    public bool _cursorVisible = false;
    public Animation animator;
    public AnimationClip fadeIn;
    public AnimationClip fadeOut;
    public AudioMixerSnapshot mute;
    public AudioMixerSnapshot unmute;
    
    [Header("Screenshot Settings")]
    public bool allowScreenshot;
    [Header("Make sure to put backslash at the end")]
    public string saveLocation = "C:\\Users\\janca\\Documents\\HKU\\Jaar 4\\Afstudeer\\Screenshots\\";
    public int resolutionMultiplier = 2;

    [HideInInspector]
    public int currentScene;
    bool _mute = false;
    bool _enableUI = true;
    GameObject canvas;

    

    private void Awake()
    {
        gameManager = this;

        currentScene = SceneManager.GetActiveScene().buildIndex;

        canvas = GameObject.Find("Canvas");

        animator.AddClip(fadeIn, "fade in");
        animator.AddClip(fadeOut, "fade out");
        animator.Play("fade in");
        StartCoroutine(SceneFadeIn(fadeIn.length));

        if (_cursorVisible)
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Screenshot") && Input.GetKey(KeyCode.LeftAlt) && allowScreenshot)
        {
            ScreenCapture.CaptureScreenshot(saveLocation + System.DateTime.Now.ToString("yyyy-MM-dd").Replace("-", ".") + "_" + System.DateTime.Now.ToString("HH-mm-ss") + "_" + SceneManager.GetActiveScene().name + "_x" + resolutionMultiplier + ".png", resolutionMultiplier);
        }
        else if (Input.GetButtonDown("Reset"))
        {
            //print("reset scene");
            animator.Play("fade out");
            StartCoroutine(SceneFadeOut(currentScene, fadeOut.length));

        }
        else if (Input.GetButtonDown("Scene Switch") && Input.GetKey(KeyCode.LeftAlt))
        {
            NextScene();
        }
        else if (Input.GetKeyDown("m"))
        {
            if (_mute)
            {
                unmute.TransitionTo(0);
                
            }
            else
            {
                mute.TransitionTo(0);
            }
            _mute = !_mute;
        }
        else if (Input.GetKeyDown("y") && Input.GetKey(KeyCode.LeftAlt))
        {
            _enableUI = !_enableUI;
            canvas.SetActive(_enableUI);
        }
        else if (Input.GetButtonDown("Quit"))
        {
            Application.Quit();
        }
    }


    public void NextScene()
    {
        if (currentScene + 1 >= SceneManager.sceneCountInBuildSettings)
        {
            animator.Play("fade out");
            StartCoroutine(SceneFadeOut(0, fadeOut.length));
        }
        else
        {
            animator.Play("fade out");
            StartCoroutine(SceneFadeOut(currentScene + 1, fadeOut.length));
        }        
    }

    public IEnumerator SceneFadeOut(int sceneNr, float duration)
    {
        float i = 0.0f;
        while ( i <= duration)
        {
            i += 0.01f;
            yield return new WaitForSeconds(0.01f);
            AudioListener.volume = Mathf.Lerp(1.0f, 0.0f, duration * i);
        }
        SceneManager.LoadScene(sceneNr);
        yield return null;
    }

    public IEnumerator SceneFadeIn(float duration)
    {
        float i = 0.0f;
        while (i <= duration)
        {
            i += 0.01f;
            yield return new WaitForSeconds(0.01f);
            AudioListener.volume = Mathf.Lerp(0.0f, 1.0f, duration * i);
        }
        yield return null;
    }

}
