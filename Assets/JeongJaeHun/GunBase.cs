using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBase : MonoBehaviour
{
    GameObject playerEquipPoint;

    BoxCollider boxCollider; //트리거가 아닌 실제 충돌체 (바닥 방지 ) 
    Rigidbody Rigidbody;

    public void PickUp(GameObject item) //player 등에서 호출 할 아이템 줍기 함수 
    {
        SetEquip(item, true);

    }

    public void Drop() //아이템 (only 총 ) 바닥에 떨어뜨리기. 
    {
        GameObject item = playerEquipPoint.GetComponentInChildren<Collider>().gameObject;
        // 이 부분 rigidbody 또는 collider로 경유해야 gameobject 불러오기 가능
        SetEquip(item, false);

        //살짝 위로 up 해서 버린다는거를 표시해주자.
        item.gameObject.transform.Translate(Vector3.up * 5f);
        playerEquipPoint.transform.DetachChildren(); //플레이어의 손에서 자식을 해제함.

    }

    public void SetEquip(GameObject item, bool isEquip)  //아이템 장착 함수 
    {
        boxCollider=item.GetComponent<BoxCollider>();
        Rigidbody=item.GetComponent<Rigidbody>();

        //true , false를 통해서 SetEquip을 부르기만 해도 끄고 키기 가능해짐. 
        boxCollider.enabled = !isEquip; //주울 때는 true 이므로 주우면 false로 콜라이더 꺼주기. 
        Rigidbody.isKinematic= isEquip; //키네마틱을 켜줘서 플레이어와 충돌되지 않도록 



    }
}
