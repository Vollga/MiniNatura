using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleCover : MonoBehaviour
{
    Material wallMat;
    float opacity = 1;
    float fadeSpeed = 0.01f;

    private void Start()
    {
        wallMat = this.GetComponent<MeshRenderer>().material;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine("WallFadeOut");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine("WallFadeIn");
        }
    }

    private IEnumerator WallFadeOut()
    {
        while( opacity >= 0)
        {
            wallMat.SetFloat("_WallOpacity", opacity);
            opacity += -fadeSpeed;
            yield return new WaitForSeconds(0.016f);
        }

    }

    private IEnumerator WallFadeIn()
    {
        while (opacity <= 1)
        {
            wallMat.SetFloat("_WallOpacity", opacity);
            opacity += fadeSpeed;
            yield return new WaitForSeconds(0.016f);
        }
    }

}
