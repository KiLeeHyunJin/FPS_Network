using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : IKWeapon
{

    public enum BombType
    {
        GRENADE, FLASHBANG
    }

    public BombType bombType;

    [Tooltip("ui에 표현될 폭탄의 스프라이트")]
    public Sprite bombSprite;
    [Tooltip("폭탄의 이름")]
    public string bombName;

    [Tooltip("폭탄의 총 갯수")]
    public int fullBombNumber;
    [Tooltip("현재 남은 폭탄의 갯수")]
    public int currentBombNumber;
    [Tooltip("폭탄의 궤적을 나타낼 라인렌더러")]
    public LineRenderer lineRenderer;

    [Header("기타 이펙트 및 애니메이션")]
    public AudioClip bomb_Sound; //폭탄 종류에 따른 사운드
    [Tooltip("폭발 이펙트")]
    public ParticleSystem bomb_ParticleSystem;

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