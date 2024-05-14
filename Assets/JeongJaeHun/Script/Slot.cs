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


    



}
