using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    public Vector2 MoveValue { get; private set; }
    public float MoveY { get; private set; }
    Action[] actions;
    Action<Vector2> moveAction;
    Action<bool> moveTypeAction;
    public InputWeaponType CurrentWeapon { get; private set; }
    public Define.FireType MainFireType { get; private set; }
    public Define.FireType Fire { get; private set; }
    public Define.FireType ChangeFireType
    {
        set
        {
            Fire = value;
            if (CurrentWeapon == InputWeaponType.MainWeapon)
                MainFireType = Fire;
        }
    }
    public void SetKey(Action method, Define.Key key)           => actions[(int)key] = method;
    public void SetMoveKey(Action<Vector2> moveMethod)          => moveAction = moveMethod;
    public void SetMoveType(Action<bool> moveTypeMethod)        => moveTypeAction = moveTypeMethod;
    private void Awake()
    {
        actions = new Action[(int)Define.Key.END];

    }
    public void Init()
    {
        InputActionAsset inputs = GetComponent<PlayerInput>().actions;
        ChangeFireType = Define.FireType.Repeat;
    }
    void OnMove(InputValue inputValue)
    {
        Vector2 MoveValue = inputValue.Get<Vector2>().normalized;
        moveAction.Invoke(MoveValue);
        MoveY = MoveValue.y;
    }
    void OnJump(InputValue inputValue)
    {
        actions[(int)Define.Key.Space].Invoke();
    }

    void OnFirstWeapon(InputValue inputValue)
    {
        actions[(int)Define.Key.F1].Invoke();
        CurrentWeapon = InputWeaponType.MainWeapon;
        ChangeFireType = MainFireType;
    }
    void OnSecondWeapon(InputValue inputValue)
    {
        actions[(int)Define.Key.F2].Invoke();
        CurrentWeapon = InputWeaponType.SubWeapon;
    }
    void OnOtherWeapon(InputValue inputValue)
    {
        actions[(int)Define.Key.F3].Invoke();
        CurrentWeapon = InputWeaponType.Default;
    }
    void OnInteraction(InputValue inputValue)
    {
        actions[(int)Define.Key.F]?.Invoke();
    }
    void OnC(InputValue inputValue)
    {
        actions[(int)Define.Key.C].Invoke();
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
    void OnR(InputValue inputValue)
    {
        actions[(int)Define.Key.R].Invoke();
    }
    void OnShift(InputValue inputValue)
    {
        bool value = inputValue.isPressed ? true : false;
        moveTypeAction.Invoke(value);
    }

    void OnAlt(InputValue inputValue)
    {
        actions[(int)Define.Key.Alt]?.Invoke();
    }

    void OnFirePress(InputValue inputValue)
    {
        if (Define.FireType.One == Fire)
            return;
        if (inputValue.isPressed)
            pressCo = StartCoroutine(PressRoutine());
        else
            StopCoroutine(pressCo);
    }
    Coroutine pressCo;
    IEnumerator PressRoutine()
    {
        while(true)
        {
            actions[(int)Define.Key.Press]?.Invoke();
            Debug.Log("Repeat");
            yield return null;
        }
    }

    void OnFireOne(InputValue inputValue)
    {
        if(Define.FireType.One == Fire)
        {
            actions[(int)Define.Key.Press]?.Invoke();
            Debug.Log("One");
        }
    }

    private void OnDestroy()
    {
        PlayerInput input = GetComponent<PlayerInput>();
        if (input != null)
            Destroy(input);
    }

    public enum InputWeaponType
    {
        MainWeapon, SubWeapon, Default, FlashBang, Grenade, END
    }
}
