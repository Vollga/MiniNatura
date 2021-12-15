using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniAudio : MonoBehaviour
{
    AudioSource source;
    PlayerController player;
    AudioClip[] footsteps;
    AudioClip[] swimClips;
    AudioClip[] gruntClips;
    AudioClip[] squelch;

    // Start is called before the first frame update
    void Start()
    {
        source = this.GetComponent<AudioSource>();
        player = this.GetComponentInParent<PlayerController>();
        footsteps = player.footstepClips;
        swimClips = player.swimSound;
        gruntClips = player.gruntClips;
        squelch = player.squelchClips;
    }

    void Step()
    {
        if (player._grounded && !player._inWater && !player._climbing && !player._onMoss)
        {
            source.PlayOneShot(footsteps[Random.Range(0, footsteps.Length)]);
        }
        else if (player._grounded && !player._inWater && !player._climbing && player._onMoss)
        {
            //source.PlayOneShot(footsteps[Random.Range(0, footsteps.Length)]);
            source.PlayOneShot(squelch[Random.Range(0, squelch.Length)]);
        }
    }

    void Swim()
    {
        source.PlayOneShot(swimClips[Random.Range(0, swimClips.Length)]);
    }

    void Grunt()
    {
        if (player._onMoss)
        {
            source.PlayOneShot(squelch[Random.Range(0, squelch.Length)]);

        }
        source.PlayOneShot(gruntClips[Random.Range(0, gruntClips.Length)]);
    }
}
