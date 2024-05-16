using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestController : MonoBehaviourPun
{
    Action jumpAction;
    Action<Vector2> moveAction;
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
        photonView.RPC(FireOnMaster, RpcTarget.MasterClient);
        photonView.RPC(FireOnEffect, RpcTarget.AllViaServer);
    }

    [PunRPC]
    public void FireOnMasterRPC()
    {
        //탄알 생성
    }

    [PunRPC]
    public void FireOnEffectRPC()
    {
        //머즐 , 쉘 , 사운드
    }

    public void Hit()
    {

    }
}
