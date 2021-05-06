using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MossInteraction : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        this.transform.localScale = new Vector3(1f, 0.1f, 1f);
        StartCoroutine("returnToSize");
    }

    IEnumerator returnToSize()
    {
        while (transform.localScale.y < 1)
        {
            this.transform.localScale += new Vector3(0f, 0.03f, 0f);
            yield return new WaitForSeconds(0.05f);
        }
    }
}
