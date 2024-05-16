using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestController : MonoBehaviourPun
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

}
