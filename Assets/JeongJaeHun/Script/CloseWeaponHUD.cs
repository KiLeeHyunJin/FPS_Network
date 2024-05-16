using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseWeaponHUD : MonoBehaviour
{
    // 탄알 수 없음

    CloseWeapon CloseWeapon; //얘는 controllger 가 abstract라서.. 
    Sword currentSword;

    [SerializeField]
    private Image WeaponImage;

    // 다만 현재 무기가 어떤 무기인지 받아올 수 있는 방법이 뭐가 있을까? 
    

    private void Update()
    {
        // 다른 스크립트에서 on off 하니까 상관안해도 되나? 다른 sprite로 on할거니까? 
        CheckUi();
    }

    private void CheckUi() //무기에 따른 스프라이트만 받아오면 된다. 
    {
        currentSword=CloseWeapon.GetCloseWeapon();
        WeaponImage.sprite = currentSword.SwordImage.sprite;
    }

}
