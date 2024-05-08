using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterIdle : CharacterStateBase
{
    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {

    }
    public override void ChangeAnimParameter()
    {

    }
    public override void Exit()
    {

    }
    protected override void NextState()
    {
        ChangeState(nextState);
    }
}
