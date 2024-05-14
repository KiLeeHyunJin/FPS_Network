using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestController : MonoBehaviourPun, IPunObservable
{
    Action jumpAction;
    Action<Vector2> moveAction;
    public void SetMove(Action<Vector2> method)
    {
        moveAction = method;
    }
    public void SetJump(Action method)
    {
        jumpAction = method;
    }
    //[PunRPC]
    //public void RequestMove(float _x, float _y)
    //{
    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        photonView.RPC("Move", RpcTarget.All, _x, _y);
    //    }
    //}

    //[PunRPC]
    //public void RequestJump()
    //{
    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        photonView.RPC("Jump", RpcTarget.All);
    //    }
    //}

    //[PunRPC]
    //public void RequestFire()
    //{
    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //    }
    //}

    //[PunRPC]
    //public void RequestAtck()
    //{
    //    if(PhotonNetwork.IsMasterClient)
    //    {

    //    }
    //}


    //[PunRPC]
    //void Jump()
    //{
    //    jumpAction?.Invoke();
    //}
    //[PunRPC]
    //void Move(float _x, float _y)
    //{
    //    moveAction?.Invoke(new Vector2(_x, _y));
    //}



    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsReading)
        {

        }
        else
        {

        }
    }
}
