using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class WeaponManager : MonoBehaviour
{
    // Holder에 붙일 웨폰 매니저. 모두가 공유가능한 static 변수 존재.
    // 무기 교체 및 여러 무기들의 동작을 관리함. --> 일단 적어보고 player에 setactive로
    // 넣어두는 방법과 얼마나 차이나는지 어떻게 이용할 수 있는지 생각해보기.

    public static bool isChangeWeapon = false; //무기 교체 중복 실행 방지 (true면 무기 교체 불가 상태 )

    [SerializeField] float changeWeaponDelayTime; //무기 교체 딜레이 시간 (총 집어넣는 타임. )
    [SerializeField] float changeWeaponEndDelayTime; //무기 교체가 완전히 끝난 시점(새로운 웨폰으로 교체된 시간)

    [SerializeField]
    private Gun[] guns; //모든 종류의 총을 원소로 가지는 배열 

    [SerializeField]

}
