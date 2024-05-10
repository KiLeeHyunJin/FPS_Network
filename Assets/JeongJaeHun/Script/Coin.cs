using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour,IInteractable
{
    // 실제 생성되는 Coin 프리팹에 붙을 코인 아이템 

    public Inventory Inventory;

    public enum CoinType
    {
       Coin1,Coin2,Coin3,Coin4
    }

    int coinValue;

    private void Start()
    {
        coinValue=(int)(GetRandomEnumValue());
    }

    public CoinType GetRandomEnumValue()
    {
        var enumValues = System.Enum.GetValues(enumType: typeof(CoinType));
        return(CoinType) enumValues.GetValue(UnityEngine.Random.Range(0, enumValues.Length));
        
    }

    public void Interaction()
    {
        Inventory.GetCoin(coinValue); //랜덤으로 설정된 코인의 가치만큼 인벤토리에 코인에 추가된다. 
    }
}
