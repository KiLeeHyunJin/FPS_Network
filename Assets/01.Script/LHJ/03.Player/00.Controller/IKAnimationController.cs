using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using static UnityEngine.Rendering.DebugUI;

public class IKAnimationController
{
    readonly Rig handRig;

    readonly TwoBoneIKConstraint[] twoBoneIKConstraint;

    readonly MultiParentConstraint[] weaoponParent;

    readonly Controller owner;

    IKWeapon currentWeapon;
    AnimationController.AnimatorWeapon currentWeaponType;
    MultiParentConstraint currentWeaponParent;

    public IKAnimationController(
        Rig _handRig,
        TwoBoneIKConstraint _leftRig,
        TwoBoneIKConstraint _rightRig, 
        MultiParentConstraint _primaryParent, 
        MultiParentConstraint _subParent, 
        MultiParentConstraint _knifeParent, 
        MultiParentConstraint _throwParent,
        Controller _owner)
    {
        handRig = _handRig;
        owner = _owner;

        twoBoneIKConstraint = new TwoBoneIKConstraint[] { _leftRig, _rightRig };
        weaoponParent = new MultiParentConstraint[] { _subParent, _primaryParent, _knifeParent, _throwParent };
    }

    public void ChangeWeapon(IKWeapon _weapon)
    {
        currentWeapon = _weapon;
        Debug.Log("ChangeParent");
        ChangeWeaponWeight(currentWeapon.weaponType);
    }

    void SetWeight(int value)
    => handRig.weight = value;

    public void DequipWeapon()
    {
        owner.StartCoroutined(
            FrameEndAction(SetWeight, 0), 
            ref SetWeightco);

        owner.StartCoroutined(
            FrameParentAction(0),
            ref parentco);
    }

    public void EquipWeapon()
    {
        owner.StartCoroutined(
            FrameEndAction(SetWeight, 1),
            ref SetWeightco);

        owner.StartCoroutined(
            FrameParentAction(1), 
            ref parentco);
    }

    public void HandOn()
    {
        owner.StartCoroutined(
            FrameEndAction(SetWeight, 0),
            ref SetWeightco);

        owner.StartCoroutined(
            FrameParentWeight(1, 0),
            ref trainsitionco);
    }

    public void AimOn()
    {
        owner.StartCoroutined(
            FrameEndAction(SetWeight, 1),
            ref SetWeightco);

        owner.StartCoroutined(
            FrameParentWeight(0, 1),
            ref trainsitionco);
    }

    Coroutine trainsitionco;
    Coroutine parentco;
    Coroutine SetWeightco;

    IEnumerator FrameEndAction(Action<int> action,int value)
    {
        yield return new WaitForEndOfFrame();
        action?.Invoke(value);
    }
    IEnumerator FrameParentAction(int value)
    {
        yield return new WaitForEndOfFrame();
        currentWeaponParent.weight = value;
    }

    IEnumerator FrameParentWeight(float value1, float value2)
    {
        yield return new WaitForEndOfFrame();
        WeightedTransformArray array = currentWeaponParent.data.sourceObjects;
        array.SetWeight(0, value1);
        array.SetWeight(1, value2);
        currentWeaponParent.data.sourceObjects = array;

        if (value2 == 1)
            owner.StartCoroutined(FrameHandTarget(), ref handCo);
    }
    Coroutine handCo;

    IEnumerator FrameHandTarget()
    {
        yield return new WaitForEndOfFrame();
        twoBoneIKConstraint[(int)Direction.Left].data.target.position = currentWeapon.leftGrip.position;
        twoBoneIKConstraint[(int)Direction.Left].data.target.rotation = currentWeapon.leftGrip.rotation;

        twoBoneIKConstraint[(int)Direction.Right].data.target.position = currentWeapon.RightGrip.position;
        twoBoneIKConstraint[(int)Direction.Right].data.target.rotation = currentWeapon.RightGrip.rotation;
    }
    void ChangeWeaponWeight(AnimationController.AnimatorWeapon weaponType)
    {
        currentWeaponParent = null;
        for (int i = 0; i < weaoponParent.Length; i++)
        {
            weaoponParent[i].weight = 0;
            if ((int)weaponType == i)
            {
                currentWeaponParent = weaoponParent[i];
                HandOn();
            }
        }
    }

    enum Direction
    {
        Left,Right,END
    }
    
}
