using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract  class  CharacterStateBase : BaseState<Define.State>
{
    protected bool isTansition;
    protected Controller controller;
    protected Define.State nextState;
    public Controller Controller { set { controller = value; } }
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
        }
    }
    public abstract void ChangeAnimParameter();

    protected abstract void NextState();
}
