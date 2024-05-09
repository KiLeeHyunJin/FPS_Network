using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class EquipController : MonoBehaviourPun
{

    // 상점 구매랑 연동을 해줘야함. --> 구매를 해서 그 자신의 상태를 확인해 줄 수 있어야하는 스크립트가 있어야지. 
    // 아머 기능이 --> 데미지 경감 + 많이 맞으면 부숴짐(내구도)  
    public enum ArmorType
    {
        StandardArmor,
        LightArmor,
        MiddleArmor,
        HeavyArmor
    }

    




    public ArmorType armorType;

    private void Start()
    {
        armorType = ArmorType.StandardArmor; //시작 아머는 스탠다드 아머임.
    }




    private void Update()
    {
        
    }



    public void Fire()
    {

    }

    public void Reload()
    {

    }

    public void WeaponSwap()
    {

    }

    public int ShieldCheck(int _damage)
    {
        




        return 1;
    }

    public Define.FireType FireTypeChange()
    {

        // 연발로 많이 쏘면 반동이 심해지고 이런 느낌?
        // 
        

        return Define.FireType.Repeat;
    }
}
