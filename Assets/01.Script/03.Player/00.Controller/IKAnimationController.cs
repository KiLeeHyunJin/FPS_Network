using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class IKAnimationController
{
    Rig rig;

    Transform leftHand;
    Transform rightHand;
    Transform weaponHolder;

    IKWeapon currentWeapon;
    public IKAnimationController(Rig _rigging, Transform _left, Transform _right, Transform _weaponHolder)
    {
        rig = _rigging;
        leftHand = _left;
        rightHand = _right;
        weaponHolder = _weaponHolder;
    }

    public void ChangeWeapon(IKWeapon _weapon)
    {
        currentWeapon = _weapon;
    }

    public void CurrenWeaponDequip()
    {

    }
    public void CurrenWeaponEquip()
    {

    }
}
