using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CloseWeaponHUD : MonoBehaviour
{
    // 어케 수정할지 생각좀 해보자. 
    // 켜져 있는 근접무기에 접근하여(3번 -> slot 내부의 자식들 순회 )
    // 그 무기의 sprite에 접근하여 sprite 띄워주기. 

    [SerializeField]
    private CloseWeapon[] CloseWeaponController; //얘는 controllger 가 abstract라서.. 
    private Sword currentSword;

    [SerializeField]
    private Image WeaponImage;

    [SerializeField]
    private TextMeshProUGUI weaponText;
    

    private void Update()
    {
        // 다른 스크립트에서 on off 하니까 상관안해도 되나? 다른 sprite로 on할거니까? 
        CheckUi();
        
    }

    private void CheckUi() //무기에 따른 스프라이트만 받아오면 된다. 
    {
        for(int i=0;i<CloseWeaponController.Length;i++)
        {
            if (CloseWeaponController[i] != null)
            {
                if (CloseWeaponController[i].gameObject.activeSelf)
                {
                    currentSword = CloseWeaponController[i].GetCloseWeapon();
                }
            }
        }
        WeaponImage.sprite = currentSword.weaponSprite; //스프라이트 
        weaponText.text = currentSword.closeWeaponName; //이름 

    }

    

}
