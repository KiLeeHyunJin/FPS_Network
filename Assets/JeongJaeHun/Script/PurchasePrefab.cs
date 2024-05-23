using System.Collections;
using TMPro;
using UnityEngine;

public class PurchasePrefab : MonoBehaviour
{
    public TextMeshProUGUI sucessText;
    public TextMeshProUGUI failText;

    public InventoryController inventory;

    public Item item; // --> 여기에 price/ prefab 등 다 있는 곳 

    public ItemPickUp itemPickUp;

    private int Id;
    private int price;

    private void OnEnable()
    {
        sucessText.enabled = false;
        failText.enabled = false;
    }


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
    public void SetItemData(ItemPickUp _pickItem)
    {
        itemPickUp = _pickItem;
    }
    public void PanelClick() //패널을 클리했을 때 가져와야 하는 정보. 
    {
        // 아이템 버튼 패널을 클릭하면 가지고 있는 스크립트를 가져와서 item 형태와 id를 확인해줘야한다.
        itemPickUp = GetComponent<ItemPickUp>();
        Id = itemPickUp.item.itemID; // 아이템의 id를 저장. 
        price = itemPickUp.item.price; //아이템의 가격을 저장. 

        // 일단 정보를 저장해두고. --> 실제 구매시에 Gold를 체크해주는 방안을 실시해줘야함. 
    }

    private void CanPurchase() //구매 가능 
    {
        if (inventory == null)
            return;

        if (price < inventory.Gold) //보유 중인 골드보다 샵 아이템의 가격이 더 비싸면. 
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine); //만약 진행중인 코루틴이 있으면 중지시키고 코루틴을 실행해줘야함. 
            }
            coroutine = StartCoroutine(LackMoney());

        }
        else  //가지고 있는 골드가 많거나 똑같으면 --> 구매 성공 
        {
            inventory.LoseCoin(price); //가격 만큼 골드 빼주기. 
            if (coroutine != null)
            {
                StopCoroutine(coroutine); //만약 진행중인 코루틴이 있으면 중지시키고 코루틴을 실행해줘야함. 
            }
            coroutine = StartCoroutine(SucessPurchase());
            Manager.UI.ClosePopUpUI(); //구매 창 닫아주기. 
                                       // 프리팹을 인벤토리에 추가해줘야함. --> 내가 보유중인 아이템 목록의 최신화 

            // 인벤토리 추가 함수 부르기. 
            inventory.AddItem(item, Id);
            Debug.Log($"{item},{Id}"); //물건의 아이템 형과 id가 제대로 들어오는지 확인해보기. 

            // 무기를 가지고 있으면 해당 panel을 비활성화 해줘야하는데 어떻게 할지 모르겟다 ㅠㅠ
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

}
