using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviourPun
{
    [SerializeField] AudioClip[] signClip;
    [SerializeField] AudioClip jumpClip;
    [SerializeField] AudioClip walkClip;
    [SerializeField] AudioClip swapClip;
    AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }
    public void PlaySound(ClipType type)
    {
        AudioClip clip;
        switch (type)
        {
            case ClipType.Jump:
                clip = jumpClip;
                break;
            case ClipType.Walk:
                clip = walkClip;
                break;
            case ClipType.Swap:
                clip = swapClip;
                break;
        }
    }

    public void PlaySign()
    {

    }



    public enum ClipType
    { 
        Jump, Walk, Swap, 
    }


}
