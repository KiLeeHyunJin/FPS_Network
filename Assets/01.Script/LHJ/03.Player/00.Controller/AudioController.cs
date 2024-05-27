using Photon.Pun;
using System.Collections;
using UnityEngine;

public class AudioController : MonoBehaviourPun
{
    [SerializeField] AudioClip[] signClip;
    [SerializeField] AudioClip jumpClip;
    [SerializeField] AudioClip landClip;
    [SerializeField] AudioClip[] walkClip;
    [SerializeField] AudioClip swapClip;
    new AudioSource audio;
    ClipType currentState;
    float checkTime = 0;

    private void Awake()
    {
        audio = GetComponent<AudioSource>();
        audio.loop = true;
        audio.playOnAwake = false;
    }
    void Start()
    {
        StartCoroutine(PlayeMoveSoundRoutine());
    }
    public void SwapWeapon()
    {
        audio.PlayOneShot(swapClip);
    }

    public void PlayMoveSound(ClipType moveType)
    {
        photonView.RPC("RPC_PlayMoveSound", RpcTarget.All, (int)moveType);
    }

    [PunRPC]
    void RPC_PlayMoveSound(int input)
    {
        currentState = (ClipType)input;
        (AudioClip clip, float time) data;
        data = currentState switch
        {
            ClipType.Jump => (jumpClip, 0),
            ClipType.Walk => (walkClip[0], 0.5f),
            ClipType.Run => (walkClip[0], 0.3f),
            ClipType.Stop => (null, 0),
            ClipType.Land => (landClip, 0),
            ClipType.END => (null, 0),
            _ => (null, 0),
        };

        audio.clip = data.clip;

        if (currentState == ClipType.Land ||
            currentState == ClipType.Jump)
            audio.Play();
        checkTime = data.time;
    }

    IEnumerator PlayeMoveSoundRoutine()
    {
        float currentTime = 0;
        bool footStep = false;
        while (true)
        {
            if (currentState == ClipType.Walk ||
                currentState == ClipType.Run)
            {
                if (checkTime <= currentTime)
                {
                    footStep = !footStep;
                    audio.clip = footStep ? walkClip[0] : walkClip[1];
                    audio.Play();
                    currentTime = 0;
                }
            }
            else if (currentState == ClipType.END)
                yield break;

            currentTime += Time.deltaTime;

            yield return null;
        }
    }

    public void PlayJump()
    {
        audio.clip = jumpClip;
        audio.Play();
    }


    public enum SignType
    {

    }
    public enum ClipType
    {
        Jump, Walk, Run, Stop, Land, END
    }


}
