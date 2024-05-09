using Photon.Pun;
using UnityEngine;
using JJH;


//장비하는 아이템을 관리할 클래스 --> 총 , 칼 , 아머 
public class EquipController : MonoBehaviourPun
{
    // 이미 장착된 아이템들을 관리해주기. 
    
    // 상점 구매랑 연동을 해줘야함. --> 구매를 해서 그 자신의 상태를 확인해 줄 수 있어야하는 스크립트가 있어야지. 
    // 아머 기능이 --> 데미지 경감 + 많이 맞으면 부숴짐(내구도)  


    // 상점 구매 할 때 여기에 있는 함수를 가져오기? --> 구매하는 장비의 타입을 여기서 만드니까 
    // 그 부분을 구매하는 스크립트에서 이용해야함. 

    ItemDataSet.ArmorType armorType;
    ItemDataSet.GunType gunType;
    ItemDataSet.SwordType swordType;

    private void Start()
    {       
        armorType = ItemDataSet.ArmorType.Standard_Armor; //start 시 기본 아머 타입으로 설정 
        gunType = ItemDataSet.GunType.StandardPistol; //기본 딱총으로 설정 
        swordType = ItemDataSet.SwordType.ShortSword; // 기본 숏소드로 설정 
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

    public Define.FireType FireTypeChange() //단발 연발 바꾸기. 
    {

        // 연발로 많이 쏘면 반동이 심해지고 이런 느낌?
        // 


        return Define.FireType.Repeat; //One or Repeat 
    }
}
