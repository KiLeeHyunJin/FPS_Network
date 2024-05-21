using System.Collections;
using UnityEngine;

public class SwordController : CloseWeaponController
{
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
