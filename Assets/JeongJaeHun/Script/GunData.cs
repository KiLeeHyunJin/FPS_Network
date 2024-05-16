using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu (fileName ="Gun_Data",menuName ="Scriptable Object/Gun_Data")]
public class GunData :ScriptableObject
{
    public enum GunType
    {
        StandardGun,
        AR,
        Pistol,
        Sniper
    }

    public GunType gunType; //총 타입 ENUM 

    [SerializeField]
    private int itemId; //아이템의 id 번호 

    public int ItemID { get { return itemId; } } //스크립터블이라 SET이 없는듯?    

    [SerializeField]
    private string itemName; // 아이템의 이름 
    public string ItemName { get { return itemName; } }

    [SerializeField]
    private int damage; //총의 데미지 
    public int Damage { get { return damage; } }

    [SerializeField]
    private int ammo; //총의 탄창 수
    public int Ammo { get { return ammo; } }

    [SerializeField]
    private int price; //총의 가격 
    public int Price { get { return price; } }

    [SerializeField]
    private Sprite gunImage; //무기의 상점 이미지. 
    public Sprite GunImage { get {  return gunImage; } }

    [SerializeField]
    private GameObject gunPrefab; // 3d 프리팹 (바닥에 있을 때 ) 
    public GameObject GunPrefab { get { return gunPrefab; } }


}


