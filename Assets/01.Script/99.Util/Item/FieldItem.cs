using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class FieldItem : PooledObject
{
    ItemData itemData;
    SpriteRenderer render;
    private void Awake()
    {
        render = GetComponent<SpriteRenderer>();
    }
    public void SetItem(Define.ItemType _type, int _id, int _count, Sprite _icon)
    {
        itemData.SetItem(_id, _type, _count);
        render.sprite = _icon;
    }
    public void ResetData()
    {
        itemData.ResetData();
        render.sprite = null;
    }
    public bool MinusItem( Define.ItemType itemType, int id, int count)
    {
        if (count > itemData.count)
            return false;
        if(id == itemData.id && itemType == itemData.type)
        {
            itemData.MinusItem(count);
            return true;
        }
        return false;
    }
    public bool AddItem(Define.ItemType itemType, int id, int count)
    {
        if (id == itemData.id && itemType == itemData.type)
        {
            itemData.AddItem(count);
            return true;
        }
        return false;
        
    }



}
