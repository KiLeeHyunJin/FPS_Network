using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultyHasItem : ItemBase
{
    public int count    { get; protected set; }
    public int weight   { get; protected set; }
    public override void GetItem()
    {

    }

    public override void Used(int count)
    {
    }
}
