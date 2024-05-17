using UnityEngine;
using UnityEngine.UI;

public class CloseWeapon : IKWeapon
{
    
    //이 스크립트를 칼 오브젝트에 붙이기. layer를 Weapon 등 근접무기 관련 layer로 변경해주기. 
    //clsoeWeapon 컨트롤러를 없애고? ikweapon을 상속했기 때문에 이미.. (다중 상속 불가능)-> 여기로 기능들 옮겨주기. 

    //근접 무기 유형 
    public enum  CloseWeaponType
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

    [Tooltip("근접 무기의 공격범위 ")]
    public float range;

    [Tooltip("근접 무기의 공격력")]
    public int damage;  // 공격력

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


    [Tooltip("현재 active 된 무기")]
    public Sword currentSword;

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


}
