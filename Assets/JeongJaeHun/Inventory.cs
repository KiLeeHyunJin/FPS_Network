using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    
    // 여기서 골드 관리 및 상점 연계 (골드쓰니까)
    public int Gold { get; set; }
    public TextMeshProUGUI goldText;

    // 획득한 아이템 --> 일단 구매라고 생각하고 하자. 획득은 그냥 땅에 떨군거 주워먹으면 되서...
    // 캐릭터 trigger GUn 체크 --> 이름 가져와서 자기 이름똑같은 Holder의 자식 active 해주면됨. 
    
    public Image itemImage; // 사실 이부분은 크게 중요하지는 않은듯함. -> hud에서 스프라이트 바꿔주면되서.. 

    // private List<GameObject> slotInventory= new List<GameObject>(); --> 나중에 list 인벤토리 필요할 때 

    

    [SerializeField]
    private Slot[] slots; //슬롯 배열 



    private void Start()
    {
       slots= gameObject.GetComponentsInChildren<Slot>();

        goldText.text = $"{0}"; //시작 시에 0원으로 초기화 
    }


    public void GetCoin(int coin)
    {
        Gold += coin; //골드 추가. 
        goldText.text = $"{Gold}";

    }
    
    public void LoseCoin(int coin) //상점 아이템 구매 등
    {
        Gold -= coin;
        if (Gold < 0) Gold = 0; //최소값 0으로 제한 
        goldText.text = $"{Gold}"; //골드텍스트 초기화 
    }

    

    public void AcquireItem(Item _item)
    {
          
    }





    public void RemoveItem() //인벤토리에서 아이템을 제거해주는 함수 --> 이거 무기 버리기 함수 가져오자. 어딨더라?
    {

    }

}
