using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class Collectible : MonoBehaviour
{
    DecalProjector glowDecal;
    
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<MeshRenderer>().enabled = false;
        glowDecal = this.GetComponentInChildren<DecalProjector>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(startGlow());
            this.GetComponent<AudioSource>().Play();
        }
    }
    
    private IEnumerator startGlow()
    {
        float timer = 0f;
        float maxTimer = 2f;
        while (timer < maxTimer)
        {
            glowDecal.fadeFactor = Mathf.SmoothStep(0, 1, timer/maxTimer );
            timer += Time.deltaTime;
            print (timer/maxTimer);
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }
}
