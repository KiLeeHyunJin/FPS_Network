using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GoldCoin : MonoBehaviour,IInteractable
{
    public GoldManager GoldManager;

    public enum coinPrice //코인들의 가치를 enum으로 분류
    {
        coin10=10,
        coin30=30,
        coin50=50,    
    };

    public coinPrice GetRandomEnumValue() // enum 값 중에 랜덤값을 가지고온다. 
    {
        var enumValues = System.Enum.GetValues(enumType: typeof(coinPrice));
        return (coinPrice)enumValues.GetValue(UnityEngine.Random.Range(0, enumValues.Length));
    }


    public void Interaction() // 매개변수 int에 enum의 int값을 대입해준다. 
    {
        int coinPrce = (int)GetRandomEnumValue();

        GoldManager.GetCoin(coinPrce); 
    }


   
}
