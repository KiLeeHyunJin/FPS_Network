using System.Collections;
using System.Runtime.CompilerServices;
using UnityEditor.Rendering;
using UnityEngine;

public class CloseWeapon : IKWeapon
{
    // 이거를 gun 처럼 데이터 컨테이너로 사용하기. 


    //근접 무기 유형 
    public enum CloseWeaponType
    {
        Sword
    }

    [Tooltip("ui에 나올 무기의 Sprite")]
    public Sprite weaponSprite;

    [Tooltip("무기의 ID 넘버")]
    public int closeWeaponID;

    [Tooltip("무기의 종류 --> 일단은 sword만 존재")]
    public CloseWeaponType closeWeaponType;

    [Tooltip("근접 무기의 이름")]
    public string closeWeaponName;

    [Tooltip("근접 무기의 공격 딜레이 ")]
    public float attackDelay; //근접 무기의 공격 딜레이 

    [Tooltip("공격 활성화 시점 ")]

    public float attackDelayA; //공격 활성화 시점 --> 애니메이션에 맞춰 공격 시작 시점 정해줌
    // 함수로 만들어서 애니메이션 이벤트에 붙이는게 편하지 않나 싶긴함 생각해보기. [

    [Tooltip("공격 비활성화 시점 -->무기를 거둬들이는 애니메이션")]
    public float attackDelayB;  //공격 비활성화 시점 -> 공격 끝 애니메이션에 맞춰 실시.

    public Animator anim; //무기에 애니메이터를 붙이나?? 


    private AudioSource audioSource;


    [Tooltip("무기 휘두르는 공격 사운드")]
    public AudioClip swordSoundClip;

    [Tooltip("무기 휘두를 시 트레일 렌더러")]
    public TrailRenderer trailRenderer;

    [Header("각도 및 데미지 체크")]
    [Tooltip("근접 무기의 공격범위 ")]
    public float range;

    [Tooltip("공격 range를 적용할 공격각도 범위 ")]
    [SerializeField][Range(0, 360)] public float angle;

    [Tooltip("근접 무기의 공격력")]
    public int damage;

    [SerializeField] protected float preAngle;
    [SerializeField] protected float cosAngle;
    public float CosAngle //각도 계산 프로퍼티 
    {
        get
        {
            if (preAngle == angle)
                return cosAngle;

            preAngle = angle;
            cosAngle = Mathf.Cos(angle * 0.5f * Mathf.Deg2Rad);
            return cosAngle;
        }
    }

    // 현재 무기에 따라 실제로 실행되는 함수를 여기다 두어야하나? 

    

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();  // 소리 

    }

    //현재 무기는 currentSword 로 파악하는 중. 


    

    



}
