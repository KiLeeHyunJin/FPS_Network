using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PurchasePrefab : MonoBehaviour
{
    public TextMeshProUGUI sucessText;
    public TextMeshProUGUI failText;

    public Inventory inventory;

    public Item item; // --> 여기에 price/ prefab 등 다 있는 곳 



    public void NoPurchase() //구매 팝업에서 no 버튼 클릭 이벤트 --> 그냥 팝업 꺼주기. 
    {
        Manager.UI.ClearPopUpUI();
    }

    public void YesPurchase() //구매 팝업에서 yes 버튼 클릭 이벤트 
    {
        Manager.UI.ClearPopUpUI();

        // 텍스트 띄워주는거는 골드만 체크하면 되는 작업이기 때문에 여기서 진행
        // 인벤토리의 골드와 아이템의 가격을 모두 알고 있어야함. 

        // 아이템 데이터 관련 함수를 불러옴. 

        CanPurchase();
    }

    public void PanelClick() //패널을 클리했을 때 가져와야 하는 정보. 
    {
        item = GetComponent<Item>(); // 패널 클릭시에 샵아이템 정보를 가지고온다. 
    }



    private void CanPurchase() //구매 가능 
    {
        // if문 내부에서 골드비교 실시. -> 부족하면 골드 부족 메시지 함수 
        
        
        if(item.price<inventory.Gold) //보유 중인 골드보다 샵 아이템의 가격이 더 비싸면. 
        {
            if(coroutine!=null)
            {
                StopCoroutine(coroutine); //만약 진행중인 코루틴이 있으면 중지시키고 코루틴을 실행해줘야함. 
            }
            coroutine = StartCoroutine(LackMoney());

        }
        else  //가지고 있는 골드가 많거나 똑같으면 --> 구매 성공 
        {
            inventory.LoseCoin(item.price); //가격 만큼 골드 빼주기. 
            if (coroutine != null)
            {
                StopCoroutine(coroutine); //만약 진행중인 코루틴이 있으면 중지시키고 코루틴을 실행해줘야함. 
            }
            coroutine= StartCoroutine(SucessPurchase());
            inventory.AddItem(item, item.itemID);
            Debug.Log($"{item},{item.itemID}");

            Manager.UI.ClosePopUpUI(); //구매 창 닫아주기. 
            // 프리팹을 인벤토리에 추가해줘야함. --> 내가 보유중인 아이템 목록의 최신화 
            
            // 인벤토리 추가 함수 부르기. 
        }


    }


    Coroutine coroutine;
    IEnumerator LackMoney() //구매시 yes ->구매실패 (부족 메시지 ) 
    {
        failText.enabled = true;
        yield return new WaitForSeconds(0.5f);
        failText.enabled = false;

    }

    IEnumerator SucessPurchase() //yes -> 구매성공 
    {
        sucessText.enabled = true;
        yield return new WaitForSeconds(0.5f);
        sucessText.enabled = false;

    }

    //버튼을 눌렀을 때 호출될 함수. ( 임시 )

    public void Btn()
    {
        inventory.AddItem(item, item.itemID);
    }


}
