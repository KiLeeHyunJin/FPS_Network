using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PurchasePrefab : MonoBehaviour
{
    public TextMeshProUGUI sucessText;
    public TextMeshProUGUI failText;

    public Inventory inventory;

    public void NoPurchase() //구매 팝업에서 no 버튼 클릭 이벤트 
    {
        Manager.UI.ClearPopUpUI();
    }

    public void YesPurchase() //구매 팝업에서 yes 버튼 클릭 이벤트 
    {
        Manager.UI.ClearPopUpUI();

        // 텍스트 띄워주는거는 골드만 체크하면 되는 작업이기 때문에 여기서 진행
        // 인벤토리의 골드와 아이템의 가격을 모두 알고 있어야함. 
        
        



        // 아이템 데이터 관련 함수를 불러옴. 
    }

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


}
