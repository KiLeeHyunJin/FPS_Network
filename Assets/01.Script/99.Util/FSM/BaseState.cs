using System;
public abstract class BaseState<T> where T : Enum
{
    protected BaseStateMachine<T> stateMachine;

    public void SetStateMachine(BaseStateMachine<T> stateMachine) 
    {
        this.stateMachine = stateMachine;
    }

    protected void ChangeState(T stateEnum)
    {
        stateMachine.ChangeState(stateEnum);
    }

    public abstract void Enter();
    public abstract void Exit();
    public abstract void Transition();
    public virtual void Update() { }
    public virtual void LateUpdate() { }
    public virtual void FixedUpdate() { }

}
