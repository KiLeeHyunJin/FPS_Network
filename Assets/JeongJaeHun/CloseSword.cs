using UnityEngine;

public class CloseSword : MonoBehaviour
{
    //근접 무기 --> 어차피 칼 한종류... 

    public string closeWeaponName; 

    //근접 무기 유형 

    public enum  CloseWeaponType
    {
        Sword
    }

    public float range; // 공격범위 
    public int damage;  // 공격력
    public float attackDelay; //근접 무기의 공격 딜레이 
    public float attackDelayA; //공격 활성화 시점 --> 애니메이션에 맞춰 공격 시작 시점 정해줌
    // 함수로 만들어서 애니메이션 이벤트에 붙이는게 편하지 않나 싶긴함 생각해보기. 
    public float attackDelayB;  //공격 비활성화 시점 -> 공격 끝 애니메이션에 맞춰 실시.

    public Animator anim; //무기에 애니메이터를 붙이나?? 


    

}
