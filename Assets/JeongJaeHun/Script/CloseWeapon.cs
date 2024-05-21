using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CloseWeapon : IKWeapon
{
    // sword는 기본 변수만 가지고 있음. --> closeWeapon에서 직접이용 (gun controller나 마찬가지 기능임)
    //결국 여기서 쓰는 변수들은 currentSword의 변수들을 이용하는 것임. --> < currentSowrd. > 

    [Tooltip("현재 active 된 무기")] // 
    public Sword currentSword { get; set; }

    [Tooltip("현재 공격중인지?")]
    protected bool isAttack = false;

    [Tooltip("팔을 휘두르는 중인지?")]
    protected bool isSwing = false; //swing 중 일 때만 공격이 진행되어야함 . (애니메이션의 시간과 맞춰주기.)


    protected RaycastHit hitInfo; //현재 무기에 닿은 것들의 정보 

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
    [Tooltip("체크할 플레이어의 Layer")]
    protected LayerMask layerMask;

    [Tooltip("근접 무기의 공격범위 ")]
    public float range;

    [Tooltip("공격 range를 적용할 공격각도 범위 ")]
    [SerializeField][Range(0, 360)] protected float angle;

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

    public Sword GetCloseWeapon() { return currentSword; }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();  // 소리 

    }

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable() //어차피 처음 시작에 꺼줄꺼니가 on에서 한 번 해보자. 
    {
    
        int numOfChild = this.transform.childCount; //현재 활성화된 무기 검색. 
        for (int i = 0; i < numOfChild; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf == true)
            {
                currentSword = transform.GetChild(i).GetComponent<Sword>();
                break;
            }
        }
        trailRenderer = currentSword.GetComponent<TrailRenderer>();
        trailRenderer.emitting = false; //공격 중이 아닐 때는 트레일 렌더러를 꺼줌. 
    }

    private void Update() // 임시 확인 
    {

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            Fire();
        }

        
    }


    public void Fire()
    {
        TryAttack();
    }


    protected void TryAttack()
    {
        if (!isAttack) //어택 중이 아니면 TryAttack 시도 (실제 공격시도)
        {
            Debug.Log("트라이어택 함수");
            StartCoroutine(AttackCoroutine());

            //어택성공하면 sound 와 trail 내부에서 진행. -->코루틴 외부에서해야함 (안 맞아도 소리는 나야함)

        }
    }


    //현재 무기는 currentSword 로 파악하는 중. 

    public IEnumerator AttackCoroutine() //공격 루틴. 
    {
        Debug.Log("attack코루틴");
        isAttack = true;

        
        yield return new WaitForSeconds(currentSword.attackDelayA); //팔 돌리기 전 대기 
        isSwing = true;

        StartCoroutine(HitCoroutine());


        isSwing = false;

        yield return new WaitForSeconds(currentSword.attackDelay -
            currentSword.attackDelayA - currentSword.attackDelayB);
        isAttack = false;

    }

    Collider[] colliders = new Collider[20];

    IEnumerator HitCoroutine() //중첩코루틴 하면 해당 코루틴이 종료될 때 까지 대기하게 된다.(while 주의) 
    {
        Debug.Log("히트코루틴 ");
       // currentSword의 tranform이 이동하면 실제 트랜스폼 이동으로찍히니까 걱정말고 overlap 돌리면 된다.
       

        yield return new WaitForSeconds(currentSword.attackDelayB); //공격 비활성화 시점 --> b 

    }

    public virtual void CloseWeaponChange(CloseWeapon _closeWeapon) // 근접 무기 변경 가상 함수. 
    {
        // 사실 안 쓸 것 같기는 한대... 무기 변경은 다른 곳에서 진행중이므로. 
    }




}
