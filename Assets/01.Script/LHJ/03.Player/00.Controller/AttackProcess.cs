using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

public class AttackProcess : MonoBehaviourPun
{

    public void Fire(AnimationController.AnimatorWeapon _weaponType)
    {
        switch (_weaponType)
        {
            case AnimationController.AnimatorWeapon.Pistol:
                GunAttack(); 
                break;
            case AnimationController.AnimatorWeapon.Rifle:
                GunAttack();
                break;
            case AnimationController.AnimatorWeapon.Sword:
                SwordAttack();
                break;
        }
    }

    void GunAttack()
    {

    }

    void SwordAttack()
    { 
    
    }



}
