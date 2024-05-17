using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Bomb : IKWeapon
{
    // 폭탄에 필요한 공통 조각
    
    // controller에서 불러오는 라인 렌더러 (수류탄 궤적 --> 던지기 전에 손에 들면 궤적이 생겨야함 )
    // 던지기 기능 (리지드바디 이용 포물선 운동 )
    // 던지기를 시작할 때 --> 리지드 바디 켜주고 + 충돌 켜주고 + 터지기 시간 준비 
    // 터지기 시간되면 폭발 --> 수류탄의 폭발 이펙트 섬광탄의 섬광 이펙트 
    // --> 추가적인 작업은 포스트프로세싱 불러오기 (스크립트로 존재 )
    // 던지는 쿨 타임 및 --> 숫자가 0 이 되면 총 들고 있는 상태로 전환해주기. ?? 아니면 그냥 헛모션 나오기 할까? 


    public enum BombType
    {
        GRENADE, FLASHBANG
    }

    public BombType bombType;

    [Header("HUD와 연계될 정보들")]
    [Tooltip("ui에 표현될 폭탄의 스프라이트")]
    public Sprite bombSprite;
    [Tooltip("폭탄의 이름")]
    public string bombName;

    [Tooltip("폭탄의 총 갯수")]
    public int fullBombNumber;
    [Tooltip("현재 남은 폭탄의 갯수")]
    public int currentBombNumber;
    /*[Tooltip("폭탄의 궤적을 나타낼 라인렌더러")]
    public LineRenderer lineRenderer;  // 그런데 이거 컨트롤러에서 하고 있는거 아닌가? */

    [Header("기타 이펙트 및 애니메이션")]
    public AudioClip bomb_Sound; //폭탄 종류에 따른 사운드
    [Tooltip("폭발 이펙트")]
    public ParticleSystem bomb_ParticleSystem;


    [Header("딜레이 관련 변수들 ")]
    [Tooltip("발사 후 몇 초 후 폭발할지 딜레이")]
    public int delayTime;

    [Tooltip("폭탄 투척 딜레이 ")]
    public int throwDelay;


    private void OnEnable() 
    {
        Debug.Log("폭탄 on 입니다.");
    }


    //[Tooltip("폭발 이펙트 관련")]

    // 인보크 등 코루틴이던 다른 곳에서 실행 시켜줄? 아니면 여기서 실행할 수도 있고 생각해보자. 
    public void CountDownBomb() //폭탄 투척시 카운트 다운임. --?컨트롤러에서 투척 실행 시 이 함수를 부르자.
    {
        switch (bombType)  //자신의 타입에 따라 다른 함수 실행. 
        {
            case BombType.GRENADE:
                Grenade();
                break;
            case BombType.FLASHBANG:
                FlashBang();
                break;
        }
    }

    public void Start()  //일단 기본적으로 시작할 때 rigidbody kinematic 실시해줘야함. 
    {
            
    }




    public void Grenade() //수류탄 터질 시 발생할 함수
    {
        bomb_ParticleSystem.Play();
        

    }

    public void FlashBang() // 섬광탄 터질 시 발생할 함수 
    {
        bomb_ParticleSystem.Play();
        

    }

    protected override void Awake()
    {
        base.Awake();
    }

    



}