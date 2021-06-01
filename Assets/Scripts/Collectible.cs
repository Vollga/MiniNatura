using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.VFX;

public class Collectible : MonoBehaviour
{
    public bool _isCollected = false;
    //public bool _useDecal = false;
    public AudioClip dig;
    public AudioClip ding;

    public VisualEffect digEffect;
    public Animator burstEffect;

    //DecalProjector glowDecal;
    //Light glowLight;
    Animation seedAni;
    float targetIntensity;

    // Start is called before the first frame update
    void Start()
    {
        seedAni = this.GetComponentInChildren<Animation>();
        //glowDecal = this.GetComponentInChildren<DecalProjector>();
        //glowLight = this.GetComponentInChildren<Light>();
        //targetIntensity = glowLight.intensity;
        //glowLight.intensity = 0;
        //glowLight.enabled = false;
        //if (_useDecal)
        //{
        //    glowDecal.transform.localPosition = new Vector3(0, -0.01f, 0);
        //    glowDecal.enabled = true;
        //}

    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetButtonDown("Interact") && other.CompareTag("Player") && _isCollected == false)
        {
            //StartCoroutine(startGlow());
            seedAni.Play();
            this.GetComponent<AudioSource>().PlayOneShot(dig);
            other.GetComponent<DemoPlayerController>().collectiblesController.CollectSeed();
            this.GetComponent<SphereCollider>().enabled = false;
            _isCollected = true;
        }
    }

    public void PlayDig()
    {
        digEffect.SendEvent("Play");
    }
    
    public void PlayBurst()
    {
        //print("Fire Burst Effect");
        burstEffect.SetTrigger("Fire");
    }

    public void PlayAudio()
    { 
        this.GetComponent<AudioSource>().PlayOneShot(ding);
    }

    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _isCollected == false)
        {
            StartCoroutine(startGlow());
            this.GetComponent<AudioSource>().Play();
            other.GetComponent<DemoPlayerController>().collectiblesController.CollectSeed();
            this.GetComponent<SphereCollider>().enabled = false;
            _isCollected = true;
        }
    }
    
    
    private IEnumerator startGlow()
    {
        float timer = 0f;
        float maxTimer = 2f;
        if (!_useDecal)
        {
            glowLight.enabled = true;
        }
        while (timer < maxTimer)
        {
            if (_useDecal)
            {
                glowDecal.fadeFactor = Mathf.SmoothStep(0, 1, timer / maxTimer);
            }
            else
            {
                glowLight.intensity = Mathf.SmoothStep(0, targetIntensity, timer / maxTimer);
            }

            timer += Time.deltaTime;
            //print (timer/maxTimer);
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

    */
}
