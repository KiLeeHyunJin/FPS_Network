using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    public Vector2 MoveValue { get; private set; }
    public bool Run { get; private set; }
    public bool Around { get; private set; }
    Action[] actions;
    Action<Vector3> moveAction;

    InputActionMap inputMap;
    InputAction fireOne;
    InputAction fireRepeat;

    public Define.FireType Fire { get; private set; }
    public Define.FireType ChangeFireType
    {
        set
        {
            Fire = value;
            OnChangeFire();
        }
    }
    public void SetKey(Action method, Define.Key key)           => actions[(int)key] = method;
    public void SetMoveKey(Action<Vector3> moveMethod)          => moveAction = moveMethod;

    private void Awake()
    {
        actions = new Action[(int)Define.Key.END];
        InputActionAsset inputs = GetComponent<PlayerInput>().actions;

        inputMap = inputs.FindActionMap("Player");
        fireOne = inputMap.FindAction("FireOne");
        fireRepeat = inputMap.FindAction("FirePress");
    }
    void MakeKeyActions()
    {
        InputActionAsset inputs = GetComponent<PlayerInput>().actions;
        inputs.AddActionMap("PlayerKeyMap");
        for (int i = 0; i < actions.Length; i++)
        {
            //inputs.
        }
    }

    void OnMove(InputValue inputValue)
    {
        MoveValue = inputValue.Get<Vector2>().normalized;
        Vector3 vector3 = new Vector3(MoveValue.x, 0,MoveValue.y);
        moveAction?.Invoke(vector3);
    }
    void OnJump(InputValue inputValue)
    {
        actions[(int)Define.Key.Space]?.Invoke();
    }

    void OnFirePress(InputValue inputValue)
    {
        actions[(int)Define.Key.Press]?.Invoke();
    }
    void OnFireOne(InputValue inputValue)
    {
        actions[(int)Define.Key.Press]?.Invoke();
    }
    void OnFirstWeapon(InputValue inputValue)
    {
        actions[(int)Define.Key.F1]?.Invoke();
    }
    void OnSecondWeapon(InputValue inputValue)
    {
        actions[(int)Define.Key.F2]?.Invoke();
    }
    void OnOtherWeapon(InputValue inputValue)
    {
        actions[(int)Define.Key.F3]?.Invoke();
    }
    void OnInteraction(InputValue inputValue)
    {
        actions[(int)Define.Key.F]?.Invoke();
    }
    void OnC(InputValue inputValue)
    {
        actions[(int)Define.Key.C]?.Invoke();
    }
    void OnX(InputValue inputValue)
    {
        actions[(int)Define.Key.X]?.Invoke();
    }
    void OnZ(InputValue inputValue)
    {
        actions[(int)Define.Key.Z]?.Invoke();
    }
    void OnV(InputValue inputValue)
    {
        actions[(int)Define.Key.V]?.Invoke();
    }

    void OnShift(InputValue inputValue)
    {
        Run = inputValue.isPressed ? true : false;
    }

    void OnAlt(InputValue inputValue)
    {
        Around = inputValue.isPressed ? true : false;
        actions[(int)Define.Key.Alt]?.Invoke();
    }
    void OnChangeFire()
    {
        inputMap.Disable();
        switch (Fire)
        {
            case Define.FireType.One:
                fireOne.Enable();
                fireRepeat.Disable();
                break;
            case Define.FireType.Repeat:
                fireOne.Disable();
                fireRepeat.Enable();
                break;
        }
        inputMap.Enable();
    }




    void OnChangeFire()
    {
        inputMap.Disable();
        switch (Fire)
        {
            case Define.FireType.One:
                fireOne.Enable();
                fireRepeat.Disable();
                break;
            case Define.FireType.Repeat:
                fireOne.Disable();
                fireRepeat.Enable();
                break;
        }
        inputMap.Enable();
    }

    private void OnDestroy()
    {
        PlayerInput input = GetComponent<PlayerInput>();
        if (input != null)
            Destroy(input);
    }
}
