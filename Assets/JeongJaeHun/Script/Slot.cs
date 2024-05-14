using UnityEngine;

public class Slot : MonoBehaviour
{
    // 각 슬롯 하나 당 할 곳인데.. --> 각 슬롯에는 각각 맞는 아이템만이 들어가야 하고

    // 1번 2번 같은 경우는 시작시에 1번 2번에 기본 무기를 가지고 시작함.
    // 상점에서 구매하면 1번 구매시 -> 기존 1번 무기를 바닥에 떨구고 새로운 1번 무기로 변경함.
    // 1번은 권총 2번은 주력무기임. 

    public Item item;
    [Tooltip("자기 자신의 슬롯 인덱스--> 인덱스로 타입 판단. ")]
    public int slotIndex;


    public void AddItem(Item _item, int ID) // 매개변수로 ID 받아서 그 ID에 맞춘 자식 오브젝트 활성화 시키기. 
    {
        item = _item;

        if (item.itemType == Item.ItemType.Gun) //상점에서 주력총 구입 시 
        {
            // 슬롯중 아이디가 1번인거를 찾아서 거기의 자식의 id를 체크 
            GameObject obj1=transform.GetChild(0).gameObject; //0번 자식 --> 첫번째 자식 

            //자식은 모두 Gun을 가지고 있을 예정이기 때문에 그 Gun을 찾고 ID가 같은거를 찾아보자. 

            foreach(Transform child in obj1.transform)
            {
                if(child.gameObject.AddComponent<Gun>().gunID==ID) //id가 일치하면 
                {
                    child.gameObject.SetActive(true); 
                }
            }
        }
        else if (item.itemType == Item.ItemType.Pistol) // 상점에서 권총 구입 시 
        {
            // 슬롯 중 아이디가 2번인거를 찾아서 거기의 자식의 id 체크


        }
        else if (item.itemType == Item.ItemType.Armor) //상점에서 아머 구입 시 
        {
            // 아머는 실제 holder로 가는게 아니라 자신의 방어력을 체크해주는 방법으로 증가시켜주면된다.



            if (ID == 1) //1번 --> 애기 아머
            {

            }
            else if (ID == 2) // 2번 --> 고급 아머 
            {

            }
        }


    }




}
