using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EquipItem : ItemBase
{
    [field : SerializeField]
    public Define.Equip equipType { get; protected set; }
    public override void Used(int count) 
    {}
    public override void GetItem()
    {}
    public abstract void Dequip();
}
