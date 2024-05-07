using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract  class  CharacterStateBase : BaseState<Define.State>
{
    protected bool isTansition;
    protected CharacterController controller;
    protected Define.State nextState;
    public CharacterController Controller { set { controller = value; } }
    public override void Enter()
    {
        isTansition = false;
        nextState = Define.State.END;
        ChangeAnimParameter();
    }
    public override void Exit()
    {
    }

    public override void Transition()
    {
        if(isTansition && nextState != Define.State.END)
        {
            NextState();
        }
    }
    public abstract void ChangeAnimParameter();

    protected abstract void NextState();
}
