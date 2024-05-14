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

    [SerializeField]
    public Item item;

    public int slotIndex; 


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

    public void AddItem(Item _item,int ID) // 매개변수로 ID 받아서 그 ID에 맞춘 자식 오브젝트 활성화 시키기. 
    {
        item = _item; 

        // 슬롯 중 아이디가 1번인 거를 찾아서 거기의 자식 id를 체크 
        if(item.itemType==Item.ItemType.Pistol)
        {
            GameObject obj1 = transform.GetChild(0).gameObject; //0번 자식 --> 첫번째 자식 (첫번째 슬롯임)

            foreach (Transform child in obj1.transform)
            {
                if (child.gameObject.AddComponent<Gun>().gunID == ID) //id가 일치하면 
                {
                    child.gameObject.SetActive(true);
                }
            }


        }
        else if (item.itemType == Item.ItemType.Gun) // 상점에서 권총 구입 시 
        {
            // 슬롯 중 아이디가 2번인거를 찾아서 거기의 자식의 id 체크
            GameObject obj2 = transform.GetChild(1).gameObject; //1번 자식 -> 주력총 
            foreach (Transform child in obj2.transform)
            {
                if (child.gameObject.AddComponent<Gun>().gunID == ID) //id가 일치하면 
                {
                    child.gameObject.SetActive(true);
                }
            }


        }
        else if (item.itemType == Item.ItemType.Armor) //상점에서 아머 구입 시 --> 따로 슬롯에 넣을 필요있나? 슬롯에서 image 작업할까? 
        {
            // 아머 관련 방어력 및 내구도 작업해서 equipController에서 작업 데미지 작업 진행 
        }

    }

    public void RemoveItem() //인벤토리에서 아이템을 제거해주는 함수 --> 이거 무기 버리기 함수 가져오자. 어딨더라?
    {

    }

}
