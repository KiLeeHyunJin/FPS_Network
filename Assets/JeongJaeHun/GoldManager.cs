using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GoldManager : MonoBehaviour
{
    //골드를 여기다 저장할 수 있게 하자.
    // 그리고 상점에서 클릭할 때 여기에 접근해서
    // 골드가격과 가지고 있는 골드를 비교해서 골드가 있을 때만
    // 구매 가능하도록 하고 구매하고 골드 차감해주는 거까지 여기서 관리해주자. 
    // 관련된 ui는 shopui에 밀어넣어서 거기에 접근해서 ui를 띄워주자. 

    // 네트워크 게임이니까 나중에 PHOTON ISMINE 추가해서 자기 일 때만 GOLD 변동하고 아이템 사고 할 수 있게 해야함. 



    public ShopUIManager ShopUIManager;

    public int gold;

    public int Gold // 외부에서 만약 Gold 상태에 직접 접근할 일이 있으면 쓸 프로퍼티 
    {
        get; protected set;
    }


    private void Start()
    {
        gold = 0;   // 게임을 시작하면 골드를 0 으로 초기화 시켜줘야한다. 
        
    }


    public void GetCoin(int plusGold) //골드를 추가해준다. 
    {
        gold += plusGold;
        ShopUIManager.goldText.text = $"{gold}";
        
    }

    public void LoseCoint(int MinusCoin) // 가지고 있는 골드에서 빼준다. 
    {
       
        gold -= MinusCoin;
        if(gold < 0) gold = 0; //gold가 0 보다 작아지면 0으로 유지해준다. 
        ShopUIManager.goldText.text = $"{gold}";

    }

    public void CompareGold(int ItemPrice) //매개변수로 들어온 item의 가격과 비교하여 돈이 충분한지 아닌지 구분한다.
    {
        //클릭 시에 클릭하는 아이템이 이 함수를 불러야한다. 
        if(gold<ItemPrice) //현재 가지고 있는 골드가 아이템 가격보다 적다면 구입할 수 없습니다 popup을 띄운다. 
        {
            ShopUIManager.OnNotPurchaseText();
        }
        else //구매에 성공한다면.
        {
            gold -= ItemPrice; //골드에서 아이템 가격을 빼준다.
            ShopUIManager.goldText.text = $"{gold}"; //gold text를 업데이트 해준다. 

            // 그리고 구매에 성공하면 그 아이템 종류에 따라서 작업을 진행해줘야하는데 
            

        }
    }






   
}
