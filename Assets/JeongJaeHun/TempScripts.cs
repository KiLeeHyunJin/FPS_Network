using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempScripts : MonoBehaviour
{
    /*if (slots[slotNumber] != null) //그리고 그 자식들이 전부 꺼져있는 경우가 아니어야 하니까.
      {
          // 전부 꺼져있는 경우라면? --> 총을 다 버렸거나 수류탄 같은거를 물량을 다 써버렸을 때 

          if (slots[slotNumber].gameObject.activeSelf == true) //이미 켜져있으면 ( 그 무기를 들고 있으면 return ) 
          {
              return;
          }

          if (slots[slotNumber].notHaving == true)
          {
              return; // 슬롯의 비어있음 bool 변수가 true면 return 하고 --> 이 부분은 리스폰 시 초기화 시켜줘야함. 
          }
          // 이게 무기 내부에 무기가 다 꺼져있으면 변경되면 안되는 작업을 맨 앞에서 해줘야 할 것 같음. --> ui가 변경되는 문제가 발생함. 

          if (slotNumber == (int) slotType.PistolType || slotNumber == (int) slotType.ArType) // gun 
  {
      closeWeaponHUD.gameObject.SetActive(false);
      bombHUD.gameObject.SetActive(false);

      gunHUD.gameObject.SetActive(true);

      StartCoroutine(ChangeWeaponCoroutine());

  }
          else if (slotNumber == (int) slotType.CloseWeaponType)
  {
      closeWeaponHUD.gameObject.SetActive(true);
      bombHUD.gameObject.SetActive(false);
      gunHUD.gameObject.SetActive(false);
      StartCoroutine(ChangeWeaponCoroutine((slotType)slotNumber)); // 슬롯 번호에 따라 다른 전환작업

  }
          else if (slotNumber == (int) slotType.GrenadeType | slotNumber == (int) slotType.FlashBangType)
  {

      closeWeaponHUD.gameObject.SetActive(false);
      bombHUD.gameObject.SetActive(true);

      gunHUD.gameObject.SetActive(false);
      StartCoroutine(ChangeWeaponCoroutine((slotType)slotNumber));
  }

  int numChild = slots[slotNumber].transform.childCount;

          for (int i = 0; i<numChild; i++)
          {

              // 켜져 있는 자식이 존재한다면. 그 무기로 바꿔주는 상황을 진행해줘야함.
              if (slots[slotNumber].transform.GetChild(i).gameObject.activeSelf == true) //무기가 켜져 있으면 (즉 무기가 있는 상황)
              {
                  foreach (Slot slot in slots)
                  {
                      slot.gameObject.SetActive(false); //일단 모든 슬롯을 꺼주고. 

                      // 나중에 확인
                      // StartCoroutine(theWeaponManager.ChangeWeaponCoroutine(quickSlots[selectedSlot].
                      // item.weaponType, quickSlots[selectedSlot].item.itemName));

                  }
slots[slotNumber].gameObject.SetActive(true); // 키가 눌린 슬롯만 켜주기. 
              }
          }*/
    /* private void Update()
     {
         if (Input.GetKeyDown(KeyCode.Alpha1))
         {
             Excute(0); //여기서 스왑할 때 (공격 불가 + 재장전 불가등의 추가 작업을 진행해줘야함. )
         }
         if (Input.GetKeyDown(KeyCode.Alpha2))
         {
             Excute(1);
         }
         if (Input.GetKeyDown(KeyCode.Alpha3))
         {
             Excute(2);
         }
         if (Input.GetKeyDown(KeyCode.Alpha4))
         {
             Excute(3);
         }
         if (Input.GetKeyDown(KeyCode.Alpha5))
         {
             Excute(4);
         }

     }*/


    /*public virtual void CloseWeaponChange(CloseWeapon _closeWeapon) //근접무기 변경. 
    {
        Debug.Log("실제 히트 루틴 내부임. "); //여기 내부 수정 필요하겠다. 웨폰 매니저를 더이상 이용하고 있지 않기 때문에 ㅠㅠ 
        if (WeaponManager.currentWeapon != null)
        {
            WeaponManager.currentWeapon.gameObject.SetActive(false);
        }

        currentCloseWeapon= _closeWeapon;
        WeaponManager.currentWeapon=currentCloseWeapon.GetComponent<Transform>();
        //WeaponManager.currentWeaponAnim = currentCloseWeapon.anim;

        currentCloseWeapon.transform.localPosition = Vector3.zero; //위치 잡는걸 플레이어에서 할 수도있고 그러면 이부분 다 삭제 해주면되나?
        currentCloseWeapon.gameObject.SetActive(true);

    }*/

    /* protected bool CheckObject() //실제 공격동작을 실행하는 동안 체크할 레이캐스트 범위. 
     {
         if (Physics.Raycast(transform.position, transform.forward, out hitInfo, currentCloseWeapon.range))
         {
             return true; //범위내에 있으면 체크. 
         }
         return false;
     }
 */


    //SwordController

    /*// 팔 holder에 붙고 sword 오브젝트의 위치를 잡고 교체해주는 등의 칼과 관련된 작업을 수행한다.

    [Tooltip("활성화 여부")]
    public static bool isActivate = true;

    private void Start()
    {
        // 이 부분을 실행하면 칼을 기본으로 들고 있도록 초기화 됨. (이게 여러 무기를 기본으로 들고 있을 수 있나?)
        WeaponManager.currentWeapon = currentCloseWeapon.GetComponent<Transform>();
        // WeaponManager.currentWeaponAnim = currentCloseWeapon.anim;
    }

   *//* protected  new IEnumerator HitCoroutine() // 칼 만의 데미지 처리 
    {
        while (isSwing) //closeWeaponController의 isSwing; 
        {
            if (CheckObject()) //여기서 플레이어를 만나야 하는데 tag가 player면? 으로 해야하나?
            {
                isSwing = false;
                
            }
            yield return null;
        }
    }*//*

    public override void CloseWeaponChange(CloseWeapon _closeWeapon) //칼 만의 무기 교체 처리 
    {
        base.CloseWeaponChange(_closeWeapon);
        isActivate = true;
    }*/


}
