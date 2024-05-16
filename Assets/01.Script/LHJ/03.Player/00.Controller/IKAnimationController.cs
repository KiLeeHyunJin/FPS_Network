using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class IKAnimationController
{
    readonly Rig rig;

    readonly TwoBoneIKConstraint[] handRig;

    readonly Transform[] handTransform;
    readonly Transform weaponHolder;
    readonly Controller owner;

    IKWeapon currentWeapon;
    int SetWeight { 
        set 
        { 
            rig.weight = value;
            Debug.Log($"Set Weight Value {value}");
        } 
    }
    public IKAnimationController(Rig _rigging, TwoBoneIKConstraint _leftRig, TwoBoneIKConstraint _rightRig, Transform _left, Transform _right, Transform _weaponHolder, Controller _owner)
    {
        rig = _rigging;
        handTransform = new Transform[]{ _left, _right };
        handRig = new TwoBoneIKConstraint[] { _leftRig, _rightRig };
        weaponHolder = _weaponHolder;
        owner = _owner;
    }

    public void ChangeWeapon(IKWeapon _weapon)
    {
        currentWeapon = _weapon;
    }

    public void DequipWeapon()
    {
        owner.StartCoroutined(
            FrameEndAction((v) => { SetWeight = v; }, 0), 
            ref co);
        currentWeapon.transform.SetParent(handTransform[(int)Direction.Right]);
        currentWeapon.transform.localPosition = Vector3.zero;
    }

    public void EquipWeapon()
    {
        currentWeapon.transform.SetParent(weaponHolder);
        currentWeapon.transform.SetLocalPositionAndRotation(currentWeapon.OriginPos, currentWeapon.OriginRot);
        owner.StartCoroutined(
            FrameEndAction((v) => { SetWeight = v; }, 1),
            ref co);
    }
    Coroutine co;
    IEnumerator FrameEndAction(Action<int> action, int value)
    {
        yield return new WaitForEndOfFrame();
        action?.Invoke(value);
    }
    enum Direction
    {
        Left,Right,END
    }
}
