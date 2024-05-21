using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour,IInteractable
{
    // 실제 생성되는 Coin 프리팹에 붙을 코인 아이템 

    public InventoryController Inventory; //인벤토리 -> 플레이어의 holder 이므로 자식임 -> 그냥 인스펙터로 첨부하자. 

    // 그냥 맵에 생성해 두는 방식의 Coin 으로 하자. 

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

    public void Interaction(GameObject player)
    {
        // 플레이어 인지 체크할 필요없음 --> 어차피 플레이어가 interaction 할거니까. 
        Inventory.GetCoin(coinValue); //랜덤으로 설정된 코인의 가치만큼 인벤토리에 코인에 추가된다. 
    }
}
