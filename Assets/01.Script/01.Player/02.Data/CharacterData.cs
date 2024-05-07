using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class CharacterData
{
    GunItem[] weapons;
    DeffenceEquipment[] equipItems;
    public int bagLv { get; private set; }
    public int helmetLv { get; private set; }
    public int vestLv { get; private set; }

    public int bagValue { get; private set; }
    public int helmetValue { get; private set; }
    public int vestValue { get; private set; }
    int currenWeaponNum { get; set; }

    public Define.Weapon GetCurrentWeaponType { get; private set; }
    public CharacterInventorySystem inventory { get; private set; }
    public int CurrentWeaponNum
    {
        private get
        {
            return currenWeaponNum;
        }
        set
        {
            currenWeaponNum = value;
            if (weapons[value] != null)
                GetCurrentWeaponType = Define.Weapon.Gun;
            else
                GetCurrentWeaponType = Define.Weapon.END;
        }
    }
    private void Awake()
    {
        weapons = new GunItem[3];
        inventory = new CharacterInventorySystem();

        equipItems = new DeffenceEquipment[3];
        for (int i = 0; i < equipItems.Length; i++)
            equipItems[i] = new DeffenceEquipment(0, 0, false);
    }
    public void Reload()
    {
        int emptyCount = weapons[CurrentWeaponNum].GetEmptyBulletCount();
        int currentBullet = inventory.CheckBullet(weapons[CurrentWeaponNum].bulletType);
        if (currentBullet == 0)
            return;
        int removeBullet = emptyCount > currentBullet ? currentBullet : emptyCount;
        weapons[CurrentWeaponNum].AddBulletCount(removeBullet);
        inventory.RemoveBullet(weapons[CurrentWeaponNum].bulletType, removeBullet);
    }
    public void AddItem(int count,ItemBase item)
    {
        MultyHasItem multyItem = item as MultyHasItem;
        if(multyItem != null)
        {
            bagValue -= count * (multyItem.weight);
            inventory.AddItem(count, item);
        }
        else if(item.ItemType == Define.Item.Equip)
        {
            SetEquip(item);
        }
    }

    void SetEquip(ItemBase item)
    {
        EquipItem equip = item as EquipItem;
        if (equip == null)
            return;

        if (equip.equipType == Define.Equip.Weapon)
            SetWeapon(equip);
        else
            SetDefenseGear(equip);
    }

    void SetWeapon(EquipItem equip)
    {
        GunItem getGun = equip as GunItem;
        if (getGun == null)
            return;
        if(getGun.gunType == Define.Gun.Pistol)
        {
            if (weapons[2] != null)
                Dequip(weapons[2]);
            weapons[2] = getGun;
        }
        else
        {
            if(CurrentWeaponNum < 1)
            {
                int idx = FindBlankGunSlot();
                if (idx == -1)
                    Dequip(weapons[0]);
                weapons[idx] = getGun;
            }
            else
            {
                if (weapons[CurrentWeaponNum] != null)
                    Dequip(weapons[0]);
                weapons[CurrentWeaponNum] = getGun;
            }
        }
    }

    void SetDefenseGear(EquipItem equip)
    {
        DeffenseGearItem deffenseGear = equip as DeffenseGearItem;
        if (deffenseGear == null)
            return;
        if(equipItems[(int)deffenseGear.wearType].wear == false)
        {
            equipItems[(int)deffenseGear.wearType].lv = deffenseGear.lv;
            equipItems[(int)deffenseGear.wearType].value = deffenseGear.value;
            equipItems[(int)deffenseGear.wearType].wear = true;
        }
        else
        {
            //Dequip()
        }
    }
    void Dequip(EquipItem equipItem)
    {

    }

    int FindBlankGunSlot()
    {
        for (int i = 0; i < 2; i++)
            if (weapons[i] == null)
                return i;
        return -1;
    }
    public void MinusDefValue(int _damage, Define.Wearable wearType)
    {
        if (wearType == Define.Wearable.Bag)
            return;
        equipItems[(int)wearType].value -= _damage;
        if (equipItems[(int)wearType].value <= 0)
            equipItems[(int)wearType].wear = false;
    }
    bool GetDefGearState(Define.Wearable wearType)
    {
        if (wearType == Define.Wearable.Bag)
            return false;
        return equipItems[(int)wearType].wear;
    }
}



public struct DeffenceEquipment
{
    public DeffenceEquipment(int _lv, int _value, bool _state)
    {
        lv = _lv;
        value = _value;
        wear = _state;
    }
    public int lv;
    public int value;
    public bool wear;
}

