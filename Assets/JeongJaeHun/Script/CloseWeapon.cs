using System.Collections;
using UnityEngine;

public class CloseWeapon : IKWeapon
{
    // 이게 지금 holder에 붙어 잇으니까 이 부분에 다 붙여넣어주자.. 


    [Tooltip("현재 active 된 무기")]
    public Sword currentSword;

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

    [Tooltip("무기 공격 사운드")]
    public AudioSource audioSource;


    [Header("각도 및 데미지 체크")]
    [Tooltip("체크할 플레이어의 Layer")]
    protected LayerMask layerMask;

    [Tooltip("근접 무기의 공격범위 ")]
    public float range;

    [Tooltip("range를 적용할 angle 범위 ")]
    [Range(0, 360)] protected float angle;

    [Tooltip("근접 무기의 공격력")]
    public int damage;

    protected float preAngle;
    protected float cosAngle;
    protected float CosAngle //각도 계산 프로퍼티 
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


    public Sword GetCloseWeapon() { return currentSword; }

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable() //어차피 처음 시작에 꺼줄꺼니가 on에서 한 번 해보자. 
    {
        int numOfChild = this.transform.childCount;
        for (int i = 0; i < numOfChild; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf == true)
            {
                currentSword = transform.GetChild(i).GetComponent<Sword>();
            }
        }
    }

    protected void TryAttack()
    {
        if (!isAttack) //어택 중이 아니면 TryAttack 시도 (실제 공격시도)
        {
            StartCoroutine(AttackCoroutine());
        }
    }


    //현재 무기는 currentSword 로 파악하는 중. 

    public IEnumerator AttackCoroutine() //공격 루틴. 
    {
        isAttack = true;
        yield return new WaitForSeconds(currentSword.attackDelayA); //팔 돌리기 전 대기 
        isSwing = true;
        
        yield return new WaitForSeconds(currentSword.attackDelayB);
        isSwing = false;

        yield return new WaitForSeconds(currentSword.attackDelay-
            currentSword.attackDelayA-currentSword.attackDelayB);
        isAttack = false;

    }

    Collider[] colliders = new Collider[20]; 

    protected void AttackTiming()
    {
        // 
        int size = Physics.OverlapSphereNonAlloc(currentSword.transform.position
            , currentSword.range, colliders, layerMask);

        for(int i=0;i<size;i++)
        {
            
        }

    }

    public virtual void CloseWeaponChange(CloseWeapon _closeWeapon) // 근접 무기 변경 가상 함수. 
    {
        // 사실 안 쓸 것 같기는 한대... 무기 변경은 다른 곳에서 진행중이므로. 
    }



}
