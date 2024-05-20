using UnityEngine;
using UnityEngine.UI;

public class Sword : CloseWeapon //close 웨폰 상속 
{
    //일단 여기에 다 모아보자. 

    [Tooltip("무기 공격 궤적")]
    public TrailRenderer TrailRenderer;

    [Tooltip("활성화 여부")]
    public static bool isActivate = true;

    


    // 어택 딜레이 등은 인스펙터로 해야하나? 



    private void OnEnable()
    {
        SetUp(); //함수를 통해서 기본 데미지 세팅을 진행. 
    }

    private void SetUp()
    {

        // 일단은 인스펙터창에서 할당해 줬음. 
    }




}
