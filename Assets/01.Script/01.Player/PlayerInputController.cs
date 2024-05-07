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
    public void SetKey(Action method, Define.Key key)
    => actions[(int)key] = method;
    private void Awake()
    {
        actions = new Action[(int)Define.Key.END];
    }

    void OnMove(InputValue inputValue)
    {
        MoveValue = inputValue.Get<Vector2>().normalized;
    }
    void OnJump(InputValue inputValue)
    {
        actions[(int)Define.Key.Space]?.Invoke();
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


    private void OnDestroy()
    {
        PlayerInput input = GetComponent<PlayerInput>();
        if (input != null)
            Destroy(input);
    }
}
