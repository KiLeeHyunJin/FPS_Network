using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CloseWeaponController : MonoBehaviour
{
    [SerializeField]
    protected CloseWeapon currentCloseWeapon; //현재 장착된 근접 무기 

    protected bool isAttack = false; //현재 공격 중인지 
    protected bool isSwing = false; //팔을 휘두르는 중인지 
    // isSwing=true 일 때만 데미지를 적용해 줘야한다. 

    protected RaycastHit hitInfo; //현재 무기에 닿은 것들의 정보 

    protected void TryAttack() //EquipController의 fire에 연동하면 fire에서 input과 연결되어있다.
        // 따로 키 관련해서 처리할 필요는 없음. reload 와 swap도 마찬가지. 
    {
        if(!isAttack) //코루틴에서 변수 넣어주는 거보다 바깥에서 변수 넣어주는게 더 깔끔해보이는듯?
        {
            StartCoroutine(AttackCoroutine());
        }
    }

   
    public IEnumerator AttackCoroutine() // 공격 루틴 
    {
        isAttack = true;
        //currentCloseWeapon.anim.SetTrigger("Attack"); // 플레이어에게 붙이는건가? 액션은 플레이어가 해야할텐데.

        yield return new WaitForSeconds(currentCloseWeapon.attackDelayA); 
        //애니메이션에 이벤트 다는 방법이 더 나을 것 같긴 한대 일단 이 방법으로 실행해보자. 실제 애니 나오기전까지
        isSwing = true;
        StartCoroutine(HitCoroutine()); // 실제로 공격데미지가 들어가는 상황

        yield return new WaitForSeconds(currentCloseWeapon.attackDelayB);
        isSwing = false;

        yield return new WaitForSeconds(currentCloseWeapon.attackDelay -
            currentCloseWeapon.attackDelayA - currentCloseWeapon.attackDelayB);
        isAttack = false; 

    }

    protected bool CheckObject() //실제 공격동작을 실행하는 동안 체크할 레이캐스트 범위. 
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, currentCloseWeapon.range))
        {
            return true; //범위내에 있으면 체크. 
        }
        return false;
    }


    protected abstract IEnumerator HitCoroutine(); //추상 코루틴도 가능하네 (실제 히트 효과니까 이걸 상속해서
    // 무기 마다 이펙트나 범위 등등을 생성해주면 될듯하다. )

    public virtual void CloseWeaponChange(CloseWeapon _closeWeapon) //근접무기 변경. 
    {
       
    }





}
