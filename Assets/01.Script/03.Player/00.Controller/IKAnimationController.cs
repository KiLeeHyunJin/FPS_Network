using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class IKAnimationController
{
    Rig rig;

    Transform[] handTransform;
    TwoBoneIKConstraint[] handRig;

    Transform weaponHolder;
    IKWeapon currentWeapon;
    Controller owner;
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

    public void CurrenWeaponDequip()
    {
        rig.weight = 0;
        currentWeapon.transform.SetParent(handTransform[(int)Direction.Right]);
        currentWeapon.transform.localPosition = Vector3.zero;
    }
    public void CurrenWeaponEquip()
    {
        rig.weight = 1;
        currentWeapon.transform.SetParent(weaponHolder);
        currentWeapon.transform.localPosition = currentWeapon.OriginPos;
        currentWeapon.transform.localRotation = currentWeapon.OriginRot;
    }
    public void Swap()
    {
        owner.StartCoroutined(SwapRoutine(), ref swapCo);
    }
    Coroutine swapCo;
    IEnumerator SwapRoutine()
    {
        float time = 1;
        currentWeapon.transform.SetParent(handTransform[(int)Direction.Right]);

        while (time > 0)
        {
            time -= Time.deltaTime;
            rig.weight = time;
            yield return null;
        }
        while(time < 1)
        {
            time += Time.deltaTime;
            rig.weight = time;
            yield return null;
        }
    }

    enum Direction
    {
        Left,Right,END
    }
}
