using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CollectiblesController : MonoBehaviour
{
    public bool _enable = true;
    [HideInInspector]
    public GameObject[] seedLocations;
    public GameObject seedSpritePrefab;
    public int counter = 0;

    GameObject[] seedSprites;

    private void Awake()
    {
        this.GetComponent<Image>().enabled = false;
    }

    // Create Seed Counter
    public void CreateSeedCounter()
    {
        //find seeds
        GameObject seedGroup = GameObject.FindGameObjectWithTag("SeedGroup");
        seedLocations = new GameObject[seedGroup.transform.childCount];

        //store seeds in array
        for (int i = 0; i < seedLocations.Length; i++)
        {
            seedLocations[i] = seedGroup.transform.GetChild(i).gameObject;
        }

        seedSprites = new GameObject[seedLocations.Length];
        for (int i = 0; i < seedSprites.Length; i++)
        {
            seedSprites[i] = Instantiate(seedSpritePrefab,transform);
            //seedSprites[i].transform.SetParent(this.transform);
            seedSprites[i].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(75 * i, 0, 0);
            seedSprites[i].GetComponent<Image>().enabled = _enable;

            Color tempColor = seedSprites[i].GetComponent<Image>().color;
            tempColor.a = 0;
            seedSprites[i].GetComponent<Image>().color = tempColor;
        }
    }

    public void CollectSeed(int seedNumber)
    {
        
        seedSprites[seedNumber].GetComponent<Image>().color = Color.white;
        counter++;
    }
}
