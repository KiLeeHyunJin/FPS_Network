using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using static Define;
using static UnityEngine.Rendering.DebugUI;

public class IKAnimationController
{
    readonly Controller owner;

    readonly RigBuilder rigBuilder;
    readonly Rig handRig;

    readonly Transform[] currentWeapons;
    readonly MultiParentConstraint[] weaoponParent;
    readonly TwoBoneIKConstraint[] twoBoneIKConstraint;

    MultiParentConstraint currentWeaponParent;

    IKWeapon currentWeapon;
    int[] currentWeaponId;
    public int this[AnimationController.AnimatorWeapon weaponType] 
    { 
        get { return currentWeaponId[(int)weaponType]; } 
    }
    public IKAnimationController(
        Rig _handRig,
        RigBuilder _rigBuilder,
        TwoBoneIKConstraint _leftRig,
        TwoBoneIKConstraint _rightRig,
        MultiParentConstraint[] _Parents,
        Controller _owner)
    {
        handRig = _handRig;
        owner = _owner;
        rigBuilder = _rigBuilder;
        weaoponParent = _Parents;
        twoBoneIKConstraint = new TwoBoneIKConstraint[] { _leftRig, _rightRig };
        currentWeaponId = new int[(int)AnimationController.AnimatorWeapon.END];
        currentWeapons = new Transform[weaoponParent.Length];
        for (int i = 0; i < currentWeapons.Length; i++)
            currentWeapons[i] = weaoponParent[i].data.constrainedObject;
    }
    public void ChangeWeapon(AnimationController.AnimatorWeapon weaponType, bool isMine = true)
    {
        Transform childObj = currentWeapons[(int)weaponType].GetChild(0);
        currentWeapon = childObj.GetComponent<IKWeapon>();
        if (currentWeaponId[(int)currentWeapon.weaponType] != currentWeapon.GetInstanceID())
            EquipWeaponEnter(currentWeapon, isMine);

        ChangeWeaponWeight(currentWeapon.weaponType);

        owner.SetZoomPosition(currentWeapon.ZoomPos);
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

        for (int i = 0; i < currentWeapons.Length; i++)
        {
            currentWeapons[i].gameObject.SetActive(false);
        }

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

    void EquipWeaponEnter(IKWeapon weapon, bool isMine)
    {
        int weaponTypeNum = isMine ? (int)weapon.weaponType : 0;

        currentWeaponId[weaponTypeNum] = weapon.GetInstanceID();
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
    }

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
                weaoponParent[i].data.sourceObjects[1].transform.position = currentWeapon.WeaponPos.transform.position;
                currentWeaponParent = weaoponParent[i];
                if(currentWeapon.gameObject.activeSelf == false)
                    currentWeapon.gameObject.SetActive(true);
                currentWeaponParent.data.constrainedObject.gameObject.SetActive(true);
                HandOn();
            }
        }
    }

    enum Direction
    {
        Left,Right,END
    }
    
}
