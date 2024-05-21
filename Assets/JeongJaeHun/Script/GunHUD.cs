using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class GunHUD : MonoBehaviour
{
    // 총알 정보를 얻기 위한 스크립트 참조 
    [SerializeField]
    private GunController[] theGunController; //Holder 위치에 건 컨트롤러 삽입 및 참조 
    private Gun curretGun;

    //총알 텍스트 ui들을 담았던 이미지 ui 할당. 필요할 때 hud 할당하고 필요 없으면 비활성화 기능 추가. 
    [SerializeField]
    private GameObject go_BulletHUD;

    [SerializeField]
    private Image gunImage;

    // 총알 개수를 텍스트 ui에 반영 
    public TextMeshProUGUI textBullet;


    //다른 스크립트에서 slot을 on/off 할 때 이 관련 hud를 끄고 켜주고 하면 되니까... 
    // 그냥 이 hud 스크립트는 여기서 그냥 돌리자. (분리는 다른 스크립트에서 해주면됨. ) 

    private void Awake() 
    {
        
    }


    private void Update()
    {
        CheckUI(); // 이거 무기 종류마다 패널을 바꾸자.
        // 무기 onEnable 될 때마다 다른 패널을 꺼줘버리자. 
    }

    private void CheckUI() //수류탄일때는 어떻게 하지? 
    {
        for(int i=0;i<theGunController.Length;i++)
        {
            if (theGunController[i] != null)
            {
                if (theGunController[i].gameObject.activeSelf==true) //켜져 있는 컨트롤러를 찾아서. 
                {
                    curretGun = theGunController[i].GetGun; //현재 상태의 건 
                }
            }
        }
        // 총알 숫자 업데이트

        if(curretGun!=null)
        {
            textBullet.text = $"{curretGun.currentBulletCount} / {curretGun.maxBulletCount}";

            //건의 이미지 
            gunImage.sprite = curretGun.gunSprite;
        }

        
    }

  


}
