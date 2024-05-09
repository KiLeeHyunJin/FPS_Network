using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBase : PooledObject
{
    public int ItemId { get; protected set; }
    public Define.Item ItemType { get; protected set; }
    public abstract void Used(int count);
    public abstract void GetItem();
}
