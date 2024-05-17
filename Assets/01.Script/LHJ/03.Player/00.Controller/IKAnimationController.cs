using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

public class IKAnimationController
{
    readonly Rig rig;

    readonly TwoBoneIKConstraint[] twoBoneIKConstraint;

    readonly Transform[] handTransform;
    readonly Transform weaponHolder;

    readonly Controller owner;
    readonly RigBuilder rigBuilder;

    IKWeapon currentWeapon;

    public IKAnimationController(Rig _rigging, TwoBoneIKConstraint _leftRig, TwoBoneIKConstraint _rightRig, Transform _left, Transform _right, Transform _weaponHolder, RigBuilder _builder, Controller _owner)
    {
        rig = _rigging;
        handTransform = new Transform[]{ _left, _right };
        twoBoneIKConstraint = new TwoBoneIKConstraint[] { _leftRig, _rightRig };
        weaponHolder = _weaponHolder;
        owner = _owner;
        rigBuilder = _builder;
    }

    public void ChangeWeapon(IKWeapon _weapon)
        => currentWeapon = _weapon;

    void SetWeight(int value)
    => rig.weight = value;

    public void DequipWeapon()
    {
        currentWeapon.transform.SetParent(handTransform[(int)Direction.Right]);

        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;

        owner.StartCoroutined(
            FrameEndAction(SetWeight, false), 
            ref co);
    }

    public void EquipWeapon()
    {
        currentWeapon.transform.SetParent(weaponHolder);

        currentWeapon.transform.localPosition = currentWeapon.OriginPos;
        currentWeapon.transform.localRotation = currentWeapon.OriginRot;

        twoBoneIKConstraint[(int)Direction.Left].data.target = currentWeapon.leftGrip;
        twoBoneIKConstraint[(int)Direction.Right].data.target = currentWeapon.RightGrip;

        owner.StartCoroutined(
            FrameEndAction(SetWeight, true),
            ref co);
    }


    Coroutine co;
    IEnumerator FrameEndAction(Action<int> action,bool state)
    {
        yield return new WaitForEndOfFrame();
        int value = state ? 1 : 0;
        action?.Invoke(value);

        if(state)
        {
            rigBuilder.Build();
            rigBuilder.SyncLayers();
        }


    }

    enum Direction
    {
        Left,Right,END
    }
}
