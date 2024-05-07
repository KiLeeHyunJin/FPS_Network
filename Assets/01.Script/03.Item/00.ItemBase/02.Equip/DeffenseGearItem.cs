using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DeffenseGearItem : EquipItem
{
    [field : SerializeField]
    public Define.Wearable wearType { get; protected set; }
    public int value { get; protected set; }
    public int lv { get; protected set; }
    public override void Dequip()
    {
    }

    public override void GetItem()
    {

    }

}
