using Photon.Pun;
using UnityEngine;


//장비하는 아이템을 관리할 클래스 --> 총 , 칼 , 아머 
public class EquipController : MonoBehaviourPun
{
    // 웨폰 스왑 함수 발동시에 --> swap 컴포넌트의 변수 가져와서 스왑 방지 시켜주기. 
    ArmorManager armorManager;
    Armor currentArmor;



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
        
        currentArmor= armorManager.GetCurrentArmor(); //현재 아머 가져오기.

        currentArmor.ArmorDurability--;

        if(currentArmor.ArmorDurability > 0 )
        {
            _damage -= currentArmor.ArmorDefense; 
        }
        
        return _damage;
    }

    public Define.FireType FireTypeChange() //단발 연발 바꾸기. 
    {

        // 연발로 많이 쏘면 반동이 심해지고 이런 느낌?
        // 


        return Define.FireType.Repeat; //One or Repeat 
    }
}
