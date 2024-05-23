using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CloseWeaponController : IKWeapon, Iattackable
{
    int actorNumber; //플레이어의 ActorNumber; 
    PhotonView pv; //플레이어의 포톤 뷰. 

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

    public bool Attack()
    {
        return TryAttack();
    }
    private void OnEnable()
    {
        int numOfChild = this.transform.childCount; //현재 활성화된 무기 검색. 
        for (int i = 0; i < numOfChild; i++)
        {
            //if (transform.GetChild(i).gameObject.activeSelf == true)
            //{

            //    break;
            //}
            currentCloseWeapon = transform.GetChild(i).GetComponent<CloseWeapon>();
            trailRenderer = currentCloseWeapon.trailRenderer;
        }
        if(trailRenderer!=null)
        trailRenderer.emitting = false; //공격하지 않을 때 렌더러가 생기지 않도록 꺼주기. 
        isAttack = false;
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

        if(photonView.IsMine) //IsMine 체크가 필요한지?? 
        {
            pv = GetComponentInParent<PhotonView>(); //부모의 포톤뷰를 가져와서
            actorNumber = pv.Owner.ActorNumber; //해당 플레이어의 액터넘버. 
        }
       
        


    }

    protected bool TryAttack() //EquipController의 fire에 연동하면 fire에서 input과 연결되어있다.
                               // 따로 키 관련해서 처리할 필요는 없음. reload 와 swap도 마찬가지. 
    {
        if (!isAttack) //코루틴에서 변수 넣어주는 거보다 바깥에서 변수 넣어주는게 더 깔끔해보이는듯?
        {
            StartCoroutine(AttackCoroutine());
            return true;
        }
        else
            return false;
    }

    public bool Reload()
    {
        return false;
    }
    IEnumerator AttackCoroutine() // 공격 루틴 
    {
        isAttack = true;
        //currentCloseWeapon.anim.SetTrigger("Attack"); // 플레이어에게 붙이는건가? 액션은 플레이어가 해야할텐데.

        yield return new WaitForSeconds(currentCloseWeapon.attackDelayA);
        //애니메이션에 이벤트 다는 방법이 더 나을 것 같긴 한대 일단 이 방법으로 실행해보자. 실제 애니 나오기전까지
        isSwing = true;

        // StartCoroutine(HitCoroutine()); // 실제로 공격데미지가 들어가는 상황
        // 함수로 한 번 실행하는게 나을듯? 
        AttackTiming(actorNumber); // TEMP ; 
        
        yield return new WaitForSeconds(currentCloseWeapon.attackDelayB);
        isSwing = false;

        yield return new WaitForSeconds(currentCloseWeapon.attackDelay -
            currentCloseWeapon.attackDelayA - currentCloseWeapon.attackDelayB);
        isAttack = false;
    }

    Collider[] colliders = new Collider[20]; // overlap으로 정면 적 확인. 

    protected  IEnumerator HitCoroutine() 
    {

        yield return null; 
    }

   private void AttackTiming(int actorNumber)
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
            damagable?.TakeDamage(currentCloseWeapon.damage,actorNumber); // 데미지 주기. + 액터넘버 확인.                                                                        
        }

    }
   

    //이 부분을 장비컨트롤러의  swap 에다 넣는다면 어떨가?? 
    

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
