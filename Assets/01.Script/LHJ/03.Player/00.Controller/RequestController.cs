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
    [SerializeField] Bullet bulletPrefab;
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
        Bullet bullet = Instantiate(bulletPrefab, transform.position + transform.forward + Vector3.up, transform.rotation);
        bullet.SetData(3, 3, 0, 0);
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
