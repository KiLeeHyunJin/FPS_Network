using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CloseWeaponController : IKWeapon
{
    AudioSource audioSource;
    private RaycastHit hitInfo; //현재 무기에 닿은 것들의 정보 

    [Tooltip("현재 장착된 무기")]
    [SerializeField]
    public CloseWeapon currentCloseWeapon; //현재 장착된 근접 무기 

    public CloseWeapon GetCloseWeapon() { return  currentCloseWeapon; }

    [Tooltip("각 근접 무기의 trail renderer")]
    private TrailRenderer trailRenderer;

    [Tooltip("데미지 체크할 플레이어의 Layer")]
    private LayerMask layerMask;

    [Tooltip("현재 공격중인지?")]
    public bool isAttack = false;
    [Tooltip("팔을 휘두르는 중인지?")]
    public bool isSwing = false;
    // isSwing=true 일 때만 데미지를 적용해 줘야한다. --> 제대로 사용을 못 하고 있어... 

    [Tooltip("자신이 가진 무기 종류 관련한 배열")]
    [SerializeField]public CloseWeapon[] closeWeapons;
    
    [Tooltip("무기 종류를 저장할 딕셔너리 (이름,타입)")]
    [SerializeField]public Dictionary<string,CloseWeapon> DicCloseWeapon=new Dictionary<string, CloseWeapon> ();

    private float range;
    LayerMask layermask; 


    private void OnEnable()
    {
        int numOfChild = this.transform.childCount; //현재 활성화된 무기 검색. 
        for (int i = 0; i < numOfChild; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf == true)
            {
                currentCloseWeapon = transform.GetChild(i).GetComponent<CloseWeapon>();
                trailRenderer=currentCloseWeapon.trailRenderer;
                break;
            }
        }

        trailRenderer.emitting = false; //공격하지 않을 때 렌더러가 생기지 않도록 꺼주기. 

        range = currentCloseWeapon.range; //캐싱? 해주기 
    }

    private void Start()
    {
        audioSource=GetComponent<AudioSource>();

        // 자신의 모든 자식을 순회하면서 dic에 이름을 넣어놓기.

        for(int i=0;i< closeWeapons.Length;i++)
        {
            DicCloseWeapon.Add(closeWeapons[i].closeWeaponName, closeWeapons[i]);
        }
              
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Fire();
        }
    }


    public void Fire()
    {
        TryAttack();
    }


    protected void TryAttack() //EquipController의 fire에 연동하면 fire에서 input과 연결되어있다.
                               // 따로 키 관련해서 처리할 필요는 없음. reload 와 swap도 마찬가지. 
    {
        if (!isAttack) //코루틴에서 변수 넣어주는 거보다 바깥에서 변수 넣어주는게 더 깔끔해보이는듯?
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

        // StartCoroutine(HitCoroutine()); // 실제로 공격데미지가 들어가는 상황
        // 함수로 한 번 실행하는게 나을듯? 
        AttackTiming(); // TEMP ; 
        
        yield return new WaitForSeconds(currentCloseWeapon.attackDelayB);
        isSwing = false;

        yield return new WaitForSeconds(currentCloseWeapon.attackDelay -
            currentCloseWeapon.attackDelayA - currentCloseWeapon.attackDelayB);
        isAttack = false;

    }

   /* protected bool CheckObject() //실제 공격동작을 실행하는 동안 체크할 레이캐스트 범위. 
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, currentCloseWeapon.range))
        {
            return true; //범위내에 있으면 체크. 
        }
        return false;
    }
*/

    Collider[] colliders = new Collider[20]; // overlap으로 정면 적 확인. 

    protected  IEnumerator HitCoroutine() //추상 코루틴도 가능하네 (실제 히트 효과니까 이걸 상속해서
    {

        yield return null; 
    }

   private void AttackTiming()
    {
        int size = Physics.OverlapSphereNonAlloc(currentCloseWeapon.transform.position,
            range, colliders, layermask);
        
        for(int i=0;i<size;i++)
        {
            Vector3 dirToTarget = (colliders[i].transform.position - currentCloseWeapon.transform.position).normalized;

            if(Vector3.Dot(currentCloseWeapon.transform.position,dirToTarget) <currentCloseWeapon.CosAngle) 
            {
                continue; //범위 바깥에 존재하면 continue로 넘기기 
            }

            IDamagable damagable = colliders[i].GetComponent<IDamagable>(); //인터페이스 가져오고
            damagable?.TakeDamage(currentCloseWeapon.damage); // 데미지 주기.                                                                            
        }

    }
   

    //이 부분을 장비컨트롤러의  swap 에다 넣는다면 어떨가?? 
    public virtual void CloseWeaponChange(CloseWeapon _closeWeapon) //근접무기 변경. 
    {
        Debug.Log("실제 히트 루틴 내부임. "); //여기 내부 수정 필요하겠다. 웨폰 매니저를 더이상 이용하고 있지 않기 때문에 ㅠㅠ 
        if (WeaponManager.currentWeapon != null)
        {
            WeaponManager.currentWeapon.gameObject.SetActive(false);
        }

        currentCloseWeapon= _closeWeapon;
        WeaponManager.currentWeapon=currentCloseWeapon.GetComponent<Transform>();
        //WeaponManager.currentWeaponAnim = currentCloseWeapon.anim;

        currentCloseWeapon.transform.localPosition = Vector3.zero; //위치 잡는걸 플레이어에서 할 수도있고 그러면 이부분 다 삭제 해주면되나?
        currentCloseWeapon.gameObject.SetActive(true);

    }

    private void OnDrawGizmos()
    {
        if(currentCloseWeapon!=null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, range);

            Vector3 rightDir = Quaternion.Euler(0, currentCloseWeapon.angle * 0.5f, 0) * transform.forward;
            Vector3 leftDir = Quaternion.Euler(0, currentCloseWeapon.angle * -0.5f, 0) * transform.forward;

            Debug.DrawRay(currentCloseWeapon.transform.position, rightDir * range, Color.cyan);
            Debug.DrawRay(currentCloseWeapon.transform.position, leftDir * range, Color.cyan);
        }
    }



}
