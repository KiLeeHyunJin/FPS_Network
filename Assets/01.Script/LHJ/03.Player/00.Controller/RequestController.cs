using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestController : MonoBehaviourPun
{
    Controller controller;
    bool isMine;
    const string FireOnMaster = "FireOnMasterRPC";
    const string FireOnEffect = "FireOnEffectRPC";

    private void Awake()
    {
        controller = GetComponent<Controller>();
    }

    private void Start()
    {
        isMine = photonView.IsMine;
    }

    public void Fire()
    {
        photonView.RPC(FireOnMaster, RpcTarget.AllViaServer);
        photonView.RPC(FireOnEffect, RpcTarget.AllViaServer);
    }

    [PunRPC]
    public void FireOnMasterRPC()
    {

    }

    [PunRPC]
    public void FireOnEffectRPC()
    {
        //머즐 , 쉘 , 사운드
    }

    public bool Hit()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            return true;
        }
        return false;
    }
}
