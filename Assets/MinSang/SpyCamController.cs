using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpyCamController : MonoBehaviourPun
{
    private Camera spyCamera;

    void Start()
    {
        spyCamera = GetComponent<Camera>();
        spyCamera.enabled = false; // 초기에는 비활성화
    }

    public void ActivateSpyCam(bool isActive)
    {
        photonView.RPC("RPC_SetActive", RpcTarget.All, isActive);
    }

    [PunRPC]
    void RPC_SetActive(bool isActive)
    {
        spyCamera.enabled = isActive;
    }
}
