using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CloseWeaponHUD : MonoBehaviourPun
{
    // 어케 수정할지 생각좀 해보자. 
    // 켜져 있는 근접무기에 접근하여(3번 -> slot 내부의 자식들 순회 )
    // 그 무기의 sprite에 접근하여 sprite 띄워주기. 

    [SerializeField]
    private CloseWeaponController[] CloseWeaponController; //얘는 controllger 가 abstract라서.. 
    private CloseWeapon currentSword;

    [SerializeField]
    private Image WeaponImage;

    [SerializeField]
    private TextMeshProUGUI weaponText;
    
    private void Update()
    {
     
        if(!photonView.IsMine)
        {
            return;
        }
         
        CheckUi();
    }

    private void CheckUi() // 이 부분 로직을 어떻게 수정할지 생각해야함. --> closeWeapon 아래의 상황을 확인해야 하는지? 
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
