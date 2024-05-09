using JJH;
using System;
using UnityEngine;

[Serializable]
public class GunData : ItemDataSet,IInteractable
{
    private int damage;
    private int _ammo;

    private float _reloadTime;

    public GunType gunType;

    GameObject player;
    GameObject playerEquipPoint;

    Rigidbody Rigidbody; 


    public float ReloadTime //총 무기의 재장전에 걸리는 시간 
    {
        get { return _reloadTime; }
        set { _reloadTime = value; }
    }

    public int Damage
    {
        get { return damage; }
        set { damage = value; }
    }

    public int Ammo //총 타입 아이템의 탄알 수 
    {
        get { return _ammo; }
        set { _ammo = value; }
    }

    private void Start()
    {
        if (gunType == ItemDataSet.GunType.AR) //건타입이 ar일 경우에는 
        {
            Damage = 10;
            Ammo = 10;
            ReloadTime = 2;

        }
        else if (gunType == ItemDataSet.GunType.SMG) //smg인 경우라면
        {
            Damage = 20;
            Ammo = 20;
            ReloadTime = 2;
        }
        else if (gunType == ItemDataSet.GunType.SR) //sr 이라면 
        {
            Damage = 30;
            Ammo = 10;
            ReloadTime = 3;
        }
        else if (gunType == ItemDataSet.GunType.StandardPistol) //기본총 이라면 
        {
            Damage = 5;
            Ammo = 20;
            ReloadTime = 10;
        }

        print($"{damage},{Ammo},{ReloadTime}");

        Rigidbody = GetComponent<Rigidbody>();

    }

    public void Interaction() //총은 바닥에 떨어져 있는 거를 주울 수 있음. 
    {
        //플레이어를 찾고 플레이어에게 들어가야함. (특히 총 + 칼 --> 실제 팔에 붙여줘야하니까 )

        playerEquipPoint = GameObject.FindGameObjectWithTag("EquipPoint"); //플레이어의 장비 포인트를 찾아서. 

        if (playerEquipPoint != null)
        {
            transform.SetParent(playerEquipPoint.transform);  //어차피 플레이어에서 이 함수 불러올거니까. 
            transform.localPosition = Vector3.zero;
            transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        
        

    }

    public GunData(GunType gunType) //건 타입에 따른 데미지 생성 
    {
        damage = Damage;
        _ammo = Ammo;
        _reloadTime = ReloadTime;
    }




}

