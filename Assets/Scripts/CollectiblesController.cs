using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CollectiblesController : MonoBehaviour
{
    public GameObject[] seedLocations;
    public GameObject seedSpritePrefab;
    public int counter = 0;

    GameObject[] seedSprites;


    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Image>().enabled = false;
        seedSprites = new GameObject[seedLocations.Length];
        for (int i = 0; i < seedSprites.Length; i++)
        {
            seedSprites[i] = Instantiate(seedSpritePrefab,transform);
            //seedSprites[i].transform.SetParent(this.transform);
            seedSprites[i].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(75 * i, 0, 0);
        }
    }

    public void CollectSeed()
    {
        seedSprites[counter].GetComponent<Image>().color = Color.white;
        counter++;
    }
}
