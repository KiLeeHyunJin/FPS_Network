using Photon.Pun;
using UnityEngine;


//장비하는 아이템을 관리할 클래스 --> 총 , 칼 , 아머 
public class EquipController : MonoBehaviourPun
{


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
