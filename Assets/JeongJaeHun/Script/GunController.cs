using Photon.Pun;
using System.Collections;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class GunController : MonoBehaviourPun, Iattackable, IPunObservable
{
    // 무기 holder에 붙일 건 컨트롤러 

    [SerializeField]
    private Gun currentGun; //현재 들고 있는 총의 Gun이 할당됨. 
    [SerializeField] Controller controller;
    public Gun GetGun { get { return currentGun; } } //프로퍼티 함수 

    [SerializeField] private float currentFireRate; //이 값이 0 보다 큰 동안에는 총알이 발사되지 않음. 
    // 초기 값은 연사속도인 Gun.cs의 fireRate 

    private bool isReload = false; //재장전 중인지 확인 --> 재장전 중이 아닐 때만 발사 가능. 
    private bool isFineSightMode = false; //정조준 중인지 확인.
    // 한 번 우클릭으로 정조준 실행하면 다시 우클릭 눌러서 해제 전까지 정조준 상태 유지 --> bool 

    [SerializeField]
    private Vector3 originPos; //원래 총의 위치 (정조준 해제하면 나중에 돌아와야함. ) 
    private AudioSource audioSource; // 총 발사 소리 재생위한 오디오소스 

    private RaycastHit hitInfo; //총알의 충돌 정보

    [Tooltip("스크립트의 활성화 여부")]
    public bool isActivate { get; set; } = true;

    [Tooltip("총알이 생성 될 FirePos 위치 ")]
    [SerializeField] private Transform FirePos;

    [SerializeField] private CrossHair crossHair; //이거 기본적으로 꺼져 있어야 하나? 어떡하지. 그런데 꺼져있으면 못찾아서;; 

    int HitLayer;

    [Header("바닥이나 벽에 총탄이 부딪히면 발생할 이펙트를 위한 레이어 체크")]
    [SerializeField] int groundWallLayer;

    [Tooltip("풀 컨테이너 참조 --> 매니저생성으로 인한 파괴 방지 ")]
    [SerializeField] PoolContainer poolContainer;

    private void Awake()
    {
        //poolContainer = GameObject.FindObjectOfType<PoolContainer>();
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        //HitLayer = 1 << LayerMask.NameToLayer("HitBox");

        // 그라운드에 총이 맞으면 이펙트를 띄워주기 위한 레이어 체크
        HitLayer = (1 << 9) | (1 << 23) | ( 1 << LayerMask.NameToLayer("HitBox"));


        originPos = Vector3.zero;
    }


    private void OnEnable()   // on off 하므로 이부분에서 할당 등을 진행해야함. 
    {
        int numOfChild = transform.childCount;
        for (int i = 0; i < numOfChild; i++)
        {
            if(transform.GetChild(i) != null)
            {
                currentGun = transform.GetChild(i).GetComponent<Gun>();
                audioSource.clip = currentGun.fire_Sound;
                break;
                // true 체크를 안하기 때문에 break를 걸어줄 필요가 없음 
            }
        }
        isActivate = true;
    }
    void Start()
    {
        controller = GetComponentInParent<Controller>();
        poolContainer = FindObjectOfType<PoolContainer>();
    }
    private void OnDisable()
    {
        isActivate = false;
    }


    // 인터페이스로 상속한 인터페이스 함수 --> 실제 플레이어 클릭 시 실행 할 함수임. 
    public bool Attack()
    {
        return TryFire();
    }

    public bool Reload()
    {
        return currentGun.otherBullet > 0 ? TryReload() : false;
    }

    private void Update()
    {
        if (isActivate)
        {
            GunFireRateCalc(); //쿨타임 측정이므로 update에서 돌아가야함. --> 사실 이런 공격 쿨타임 코루틴으로 구현하면 되긴 함... 
            //TryFire(); //발사 입력 받는 부분은 update에서 굳이 돌려야할까? --> input 쓰는데? 생각해보기. 
            //TryReload(); //재장전도 마찬가지 -> 키 눌렀을 때만 측정하면 되지 않을까? 
            //TryFineSight(); //정조준 
        }
    }

    private void GunFireRateCalc() // 총의 쿨타임 
    {
        if (currentFireRate > 0)
        {
            currentFireRate -= Time.deltaTime; // deltaTime만큼 지속적으로 감소 
        }
    }

    private bool TryFire() //발사 입력을 받음. --> 이 부분 EquipController에서 관리하므로 인풋을 넣을 필요없음
    {
        if (currentFireRate <= 0 && !isReload) //쿨타임 <=0 이고 재장전 중이 아닐 때만 Fire 실행. 
        {
            if (currentGun.currentBulletCount > 0) //재장전 중이 아니면서 동시에 총알이 남아있으면 Shoot()실행. 
            {
                currentGun.currentBulletCount--; //총알 감소 
                currentFireRate = currentGun.fireRate; //연사 속도 재계산 ( deltaTime 빼줘서 0 되기전까지 다시 발사 중지)

                Vector3 firePos = controller.Zoom ?
                    currentGun.muzzleFlash.transform.position:
                    Camera.main.transform.position;

                photonView.RPC("Shoot", RpcTarget.MasterClient,
                    photonView.Controller.ActorNumber,
                    firePos,
                    controller.Zoom);

                photonView.RPC("Effect", RpcTarget.All);
                return true;
            }
            else
            {
                audioSource.PlayOneShot(currentGun.dryFire_Sound);
                //원본에서는 여기서 Reload를 실행하지만 우리 게임은 Reload를 키를 눌러서 진행할 예정이다.
            }
        }
        return false;
    }

    [PunRPC]
    private void Effect()
    {
        currentGun.muzzleFlash.Play(); //총 발사시에 이펙트 발생.      
        audioSource.Play(); //현재 gun의 fireSound 재생.
    }

    [PunRPC] //Shoot을 실제 실행하는 Attack 에서는 isMine 체크.
    private void Shoot(int ActorNumber, Vector3 pos, bool zoomState) //실제 발사되는 과정 
    {
        /*PooledObject bullet=
        Manager.Pool.GetBullet(FirePos.position, Quaternion.identity); //총구에서 총알 생성.
        bullet.GetComponent<Bullet>().actorNumber = ActorNumber; //pool로 */
        Vector3 dir = zoomState ?
            currentGun.transform.forward :
            (controller.target.position - pos).normalized;// : 
            //(controller.target.position - currentGun.muzzleFlash.transform.position).normalized;
        Debug.DrawLine(pos + dir, pos + dir * currentGun.range, Color.cyan, 2);


        if (Physics.Raycast(
            pos + dir,
            dir,
            out hitInfo,
            currentGun.range, HitLayer))
        {
            bool ishit = false;
            if (hitInfo.collider.TryGetComponent<IDamagable>(out IDamagable damagable))
            {
                Debug.Log($"Hit Damage {currentGun.damage} ");

                damagable.TakeDamage(currentGun.damage, ActorNumber); //actorNumber가 laycast의 주인 actorNumber 
                ishit = true;
                /*poolContainer.GetBloodEffect(hitInfo.point, Quaternion.LookRotation(hitInfo.normal));*/
            }
            else
            {
                /*poolContainer.GetbulletMarks(hitInfo.point - (hitInfo.normal * 0.1f), Quaternion.LookRotation(hitInfo.normal));
                poolContainer.GetBulletSpark(hitInfo.point, Quaternion.LookRotation(-hitInfo.normal));*/
            }
            photonView.RPC("HitEffect", RpcTarget.All,hitInfo.point, hitInfo.normal,ishit);
        }
    }

    [PunRPC]
    void HitEffect(Vector3 pos, Vector3 nor, bool isHit)
    {
        if (isHit)
            poolContainer.GetBloodEffect(pos, Quaternion.LookRotation(nor));
        else
        {
            poolContainer.GetbulletMarks(pos + (nor * 0.1f), Quaternion.LookRotation(nor));
            poolContainer.GetBulletSpark(pos, Quaternion.LookRotation(-nor));
        }
    }


    private bool TryReload() //리로드 또한 장비컨트롤러에서 실제 키와 연결되어 있으므로 인풋 제한 걸 필요없다.
    {
        if (!isReload)
        {
            //CancelFineSight(); //정조준 상태 해제 후 리로드 시작. 
            StartCoroutine(ReloadCoroutine());
            return true;
        }
        return false;
    }

    IEnumerator ReloadCoroutine() //재장전 코루틴. 
    {
        if (currentGun.currentBulletCount < currentGun.maxBulletCount) //max숫자는 변동없음. 
        {
            //현재 총탄의 갯수가 최대 총탄의 갯수보다 적으면 리로딩 진행
            isReload = true;
            currentGun.otherBullet += currentGun.currentBulletCount;
          
            //currentGun.anim.SetTrigger("Reload"); 여기서 애니메이션을 수행해야 된다면 재생.
            yield return new WaitForSeconds(currentGun.reloadTime); //재장전 애니메이션 동안 대기 
            if (currentGun.maxBulletCount <= currentGun.otherBullet)
            {
                currentGun.otherBullet -= currentGun.maxBulletCount;
                currentGun.currentBulletCount = currentGun.maxBulletCount; //현재 총탄을 최대총탄 숫자로 맞춰줌.
            }
            else
            {
                currentGun.currentBulletCount = currentGun.otherBullet;
                currentGun.otherBullet = 0;
            }
            isReload = false;
            audioSource.PlayOneShot(currentGun.reload_Sound);
        }
    }


    private void FineSight()
    {
        isFineSightMode = !isFineSightMode; //bool 상태 전환 
                                            //currentGun.anim.SetBool("FindSightMode", isFindSightMode); //bool로 관련 애니메이션 전환. 

        if (isFineSightMode) //정조준 모드 진입 
        {
            StopAllCoroutines(); //모든 코루틴 중지 -->while 돌고있는 DeActive를 중지하기 위함. 
            StartCoroutine(FindSightActivateCoroutine());
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(FindSightDeActivateCoroutine());
        }
    }

    IEnumerator FindSightActivateCoroutine() //정조준 실행 코루틴 
    {
        while (currentGun.transform.localPosition != currentGun.fineSightOriginPos)
        {
            //자신의 총의 위치를 정조준시 총의 위치로 변화시킴. --?Lerp 이용 
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition
                , currentGun.fineSightOriginPos, 0.2f);
            yield return null;
        }
    }

    IEnumerator FindSightDeActivateCoroutine() //정조준 해제 코루틴 
    {
        while (currentGun.transform.localPosition != originPos)
        // 원래의 위치로 보간을 통해 조정 --> 총의 실제 위치를 조절하는 부분임... 
        {
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition
                , originPos, 0.2f);
            yield return null;
        }
    }

    // 반동 구현 코루틴 
    IEnumerator RetroActionCoroutine()
    {
        //recoil : 움찔하다.  -> 정조준 안 했을 때의 최대반동 
        Vector3 recoilBack = new Vector3(currentGun.retroActionForce, originPos.y, originPos.z);
        // x 만큼 총 이동 but 이 부분은 실제 GunHolder의 회전 상태에 따라 다르다. 
        // 총의 앞뒤 이동이 x 인지 z 인지 혹은 다른 방향인지 실제로 넣어보고 판단해야 한다. 
        Vector3 retroActionRecoilBack
            = new Vector3(currentGun.retroActionFineSightForce, currentGun.fineSightOriginPos.y,
            currentGun.fineSightOriginPos.z); //정조준 했을 때의 최대 반동 

        if (!isFineSightMode) //정조준이 아닌 상태 --> 정조준되면 에임 크기도 변해야 할텐데.. 
        {
            currentGun.transform.localPosition = originPos; //총의 로컬 위치를 originPos로 설정
            while (currentGun.transform.localPosition.x <= currentGun.retroActionForce - 0.02f)
            {
                // 총의 x 방향 위치가 recoilBack 보다 커질 때가지 반복 -> lerp로 인해서 영원히 도달 못할 수 있기에
                // recoilBack 에서 0.02를 빼준 값보다 커질 때 까지만 반복하도록 보정해줌. 
                currentGun.transform.localPosition = Vector3.Lerp
                    (currentGun.transform.localPosition, recoilBack, 0.4f);
                yield return null;
            }
            //반동 시작 while문 탈출한 이후 다시 제자리로 돌아가는 while문을 재생해줌. 

            while (currentGun.transform.localPosition != originPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp
                    (currentGun.transform.localPosition, originPos, 0.1f);
                yield return null;
            }
        }
        else //정조준 상태 
        {
            currentGun.transform.localPosition = currentGun.fineSightOriginPos;

            // 반동 시작 --> 동일한 루틴 but 반동값과 정조준 위치만 다름. 
            while (currentGun.transform.localPosition.x <= currentGun.retroActionFineSightForce - 0.02f)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, retroActionRecoilBack, 0.4f);
                yield return null;
            }

            // 원위치
            while (currentGun.transform.localPosition != currentGun.fineSightOriginPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.1f);
                yield return null;
            }
        }


    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}
