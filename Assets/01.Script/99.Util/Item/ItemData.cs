using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public struct ItemData
{
    public int id { get; private set; }
    public int count { get; private set; }
    public Define.ItemType type { get; private set; }
    public void ResetData()
    {
        id = -1;
        count = -1;
        type = Define.ItemType.Ect;
    }
    public void SetItem( int _id, Define.ItemType _type, int _count)
    {
        id = _id;
        count = _count;
        type = _type;
    }
    public int AddItem(int _count) => count += _count;
    public bool MinusItem(int minusItemCount)
    {
        if (count < minusItemCount)
            return false;
        count -= minusItemCount;
        return true;
    }
}
