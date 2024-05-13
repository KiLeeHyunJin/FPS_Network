using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    public Vector2 MoveValue { get; private set; }
    public float MoveY { get; private set; }
    Action[] actions;
    Action<Vector2> moveAction;
    Action<Vector2> rotateAction;
    Action<bool> moveTypeAction;
    public Define.InputWeaponType CurrentWeapon { get; private set; }
    public Define.FireType MainFireType { get; private set; }
    public Define.FireType Fire { get; private set; }
    public Define.FireType ChangeFireType
    {
        set
        {
            Fire = value;
            if (CurrentWeapon == Define.InputWeaponType.MainWeapon)
                MainFireType = Fire;
        }
    }
    public void SetKey(Action method, Define.Key key) => actions[(int)key] = method;
    public void SetMoveKey(Action<Vector2> moveMethod) => moveAction = moveMethod;
    public void SetRot(Action<Vector2> rotMethod) => rotateAction = rotMethod;
    public void SetMoveType(Action<bool> moveTypeMethod) => moveTypeAction = moveTypeMethod;

    public void Init()
    {
        //InputActionAsset inputs = GetComponent<PlayerInput>().actions;
        ChangeFireType = Define.FireType.Repeat;
        actions ??= new Action[(int)Define.Key.END];
    }
    void OnLook(InputValue inputValue)
    {
        rotateAction?.Invoke(inputValue.Get<Vector2>());
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
        CurrentWeapon = Define.InputWeaponType.MainWeapon;
        ChangeFireType = MainFireType;
    }
    void OnSecondWeapon(InputValue inputValue)
    {
        actions[(int)Define.Key.F2].Invoke();
        CurrentWeapon = Define.InputWeaponType.SubWeapon;
    }
    void OnOtherWeapon(InputValue inputValue)
    {
        actions[(int)Define.Key.F3].Invoke();
        CurrentWeapon = Define.InputWeaponType.Default;
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
        while (true)
        {
            actions[(int)Define.Key.Press]?.Invoke();
            yield return null;
        }
    }

    void OnFireOne(InputValue inputValue)
    {
        if (Define.FireType.One == Fire)
        {
            actions[(int)Define.Key.Press]?.Invoke();
            Debug.Log("One");
        }
    }

    void OnDestroy()
    {
        PlayerInput input = GetComponent<PlayerInput>();
        if (input != null)
            Destroy(input);
    }


}
