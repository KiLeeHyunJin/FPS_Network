using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItemSystem
{
    ItemData[] items;
    public void SetCount(int count) => items = new ItemData[count];
    public int Count => items.Length;

    public void AddItem(int id,Define.ItemType itemType, int count)
    {
        int idx = FindItem(id, itemType);
        if(idx > -1)
            items[idx].AddItem(count);
        else
        {
            idx = BlanckIndex();
            if (idx < 0)
                return;
            items[idx].SetItem(id, itemType, count);
        }
    }
    public void MinusItem(int id, Define.ItemType itemType, int count)
    {
        int idx = FindItem(id, itemType);
        if (idx > -1)
        {
            if(items[idx].MinusItem(count)&&
               items[idx].count <= 0)
            {
               items[idx].ResetData();
            }
        }
    }
    int FindItem(int id, Define.ItemType itemType)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].type == itemType &&
                items[i].id == id)
            { 
                return i;
            }
        }
        return -1;
    }
    public int BlanckIndex()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].id == -1)
            {
                return i;
            }
        }
        return -1;
    }

}
