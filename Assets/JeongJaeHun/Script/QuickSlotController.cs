using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeWeaponSlot : MonoBehaviour
{
    // 어차피 부모 inventory가 켜져 잇으니까 그냥 slot을 껏다 켯다 하는식으로하면
    // 그 내부의 무기들을 active 하는거는 다른 곳에서 작업하기 때문에 
    // 그냥 여기서는 큰 슬롯들을 켰다 껏다 하는 방식만 사용하면 된다. 

    [SerializeField] private Slot[] slots; //무기 슬롯 배열 

    [SerializeField] private WeaponManager weaponManager;

    Inventory inventory;


    private void Start()
    {
        inventory=GameObject.FindObjectOfType<Inventory>();
        slots=inventory.GetComponentsInChildren<Slot>();
        foreach(Slot slot in slots)
        {
            slot.gameObject.SetActive(false); //일단 모든 슬롯을 꺼주고. 
        }

        slots[0].gameObject.SetActive(true); //0번 슬롯 --> 즉 피스톨 부분만 start 시에 켜줘서 시작해주기. 
    }

    // 무기 버리는 경우가 있을 수 있으니까 만약 slot 내부에 모든 종류의 아이템이 꺼져 있다면 
    // 그 슬롯으로 전환이 불가능한 경우이므로 .. 전환을 return 해주자. 

    public void Excute(int slotNumber) //실제 전환 실행 --> equip컨트롤에서 불러서 작업해주기. 
    {
        // 슬롯 내부의 자식들을 체크해서 그 자식들이 켜져 있는지 확인하고 하나도 안켜져 있다면 return 해버리기
 
        if (slots[slotNumber] != null) //그리고 그 자식들이 전부 꺼져있는 경우가 아니어야 하니까.
        {
            // 전부 꺼져있는 경우라면? --> 총을 다 버렸거나 수류탄 같은거를 물량을 다 써버렸을 때 

            int numChild = slots[slotNumber].transform.childCount;
            for(int i=0;i<numChild;i++)
            {
                // 켜져 있는 자식이 존재한다면. 그 무기로 바꿔주는 상황을 진행해줘야함.
                if (transform.GetChild(i).gameObject.activeSelf == true) //무기가 켜져 있으면 
                {
                    foreach (Slot slot in slots)
                    {
                        slot.gameObject.SetActive(false); //일단 모든 슬롯을 꺼주고. 
                        slots[slotNumber].gameObject.SetActive(true); // 키가 눌린 슬롯만 켜주기. 

                        // 나중에 확인
                        // StartCoroutine(theWeaponManager.ChangeWeaponCoroutine(quickSlots[selectedSlot].
                        // item.weaponType, quickSlots[selectedSlot].item.itemName));

                    }
                }

            }

        }
    }







}
