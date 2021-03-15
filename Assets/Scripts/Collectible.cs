using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class Collectible : MonoBehaviour
{
    public bool _isCollected = false;
    public bool _useDecal = false;

    DecalProjector glowDecal;
    Light glowLight;

    float targetIntensity;

    // Start is called before the first frame update
    void Start()
    {
        glowDecal = this.GetComponentInChildren<DecalProjector>();
        glowLight = this.GetComponentInChildren<Light>();
        targetIntensity = glowLight.intensity;
        glowLight.intensity = 0;
        glowLight.enabled = false;
        if (_useDecal)
        {
            glowDecal.transform.localPosition = new Vector3(0, -0.01f, 0);
            glowDecal.enabled = true;
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _isCollected == false)
        {
            StartCoroutine(startGlow());
            this.GetComponent<AudioSource>().Play();
            other.GetComponent<PlayerController>().collectiblesController.CollectSeed();
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
}
