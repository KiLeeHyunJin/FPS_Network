using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class EquipController : MonoBehaviourPun
{

//데이터 보관해주기. 
    public static class EquipData
    {
        public enum ArmorTypeEnum
        {
            StandardArmor,
            LightArmor,
            MiddleArmor,
            HeavyArmor
        }

        public struct ArmorType
        {
        
        }
    }

    // 캐릭터마다 하나씩 들어야 되는 것들은 singleton을 하면 안된다. 

    // 상점 구매랑 연동을 해줘야함. --> 구매를 해서 그 자신의 상태를 확인해 줄 수 있어야하는 스크립트가 있어야지. 
    // 아머 기능이 --> 데미지 경감 + 많이 맞으면 부숴짐(내구도)  
    

    // 상점 구매 할 때 여기에 있는 함수를 가져오기? --> 구매하는 장비의 타입을 여기서 만드니까 
    // 그 부분을 구매하는 스크립트에서 이용해야함. 



    private void Start()
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
