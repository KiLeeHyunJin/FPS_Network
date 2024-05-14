using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class PurchaseAction : MonoBehaviour

    // 버튼에 다는 스크립트 (구매 액션 )
{
    private Inventory inventory;

    private ItemPickUp itemPickUp;

    int Id;
    int price;




    private void Purchase() // 상점에서 클릭하는 이벤트 (버튼 이벤트 )
    {
        itemPickUp=GetComponent<ItemPickUp>();  // 컴포넌트 가져오고. -> 
        Id = itemPickUp.item.itemID;  // 아이템의 id 
        price = itemPickUp.item.price;  // 아이템의 price 

        inventory.AddItem(itemPickUp.item, Id); // 가격은 어차피 다른데서 확인하지 않나? 여기에 붙일까?



    }





}
