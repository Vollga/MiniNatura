using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniAudio : MonoBehaviour
{
    AudioSource source;
    PlayerController player;
    AudioClip[] footsteps;

    // Start is called before the first frame update
    void Start()
    {
        source = this.GetComponent<AudioSource>();
        player = this.GetComponentInParent<PlayerController>();
        footsteps = player.footstepClips;
    }

    void Step()
    {
        if (player._grounded)
        {
            source.PlayOneShot(footsteps[Random.Range(0, footsteps.Length)]);
        }
    }
}
