using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseableItem : MultyHasItem
{
    public Define.Heal healType { get; protected set; }
    public int healValue { get; protected set; }
    public float consumTime { get; protected set; }

}
