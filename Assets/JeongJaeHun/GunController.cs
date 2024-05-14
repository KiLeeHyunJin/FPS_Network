using System.Collections;
using UnityEngine;

public class GunController : MonoBehaviour
{
    // 무기 holder에 붙일 건 컨트롤러 

    [SerializeField]
    private Gun currentGun; //현재 들고 있는 총의 Gun이 할당됨. 

    public Gun GetGun() { return currentGun; } //프로퍼티 함수 

    private float currentFireRate; //이 값이 0 보다 큰 동안에는 총알이 발사되지 않음. 
    // 초기 값은 연사속도인 Gun.cs의 fireRate 

    private bool isReload = false; //재장전 중인지 확인 --> 재장전 중이 아닐 때만 발사 가능. 
    private bool isFineSightMode = false; //정조준 중인지 확인.
    // 한 번 우클릭으로 정조준 실행하면 다시 우클릭 눌러서 해제 전까지 정조준 상태 유지 --> bool 
    [SerializeField]
    private Vector3 originPos; //원래 총의 위치 (정조준 해제하면 나중에 돌아와야함. ) 
    private AudioSource audioSource; // 총 발사 소리 재생위한 오디오소스 

    private RaycastHit hitInfo; //총알의 충돌 정보
    [SerializeField]
    private Camera theCam; //카메라 시점에서 정 중앙에 발사할 것. 

    [SerializeField]
    [Tooltip("피격 시 발생할 피 터지는 이펙트")]

    [Header("PooledObject 이펙트 관리 스크립트연결")]
    public PoolContainer poolContainer;

    [Tooltip("스크립트의 활성화 여부")]
    public static bool isActivate = true;
    private void Awake()
    {
        poolContainer = GameObject.FindObjectOfType<PoolContainer>();
    }


    private void Start()
    {
        originPos = Vector3.zero;
        audioSource=GetComponent<AudioSource>();
        WeaponManager.currentWeapon=currentGun.GetComponent<Transform>();
        //기본적인 디폴트 무기를 Gun으로 삼기 위해 currentWeapon에 자기 자신의 transform을 할당해줌
        audioSource = GetComponent<AudioSource>();
        //WeaponManager.currentWeaponAnim=currentGun.anim;
    }

    private void Update()
    {
        if(isActivate)
        {
            GunFireRateCalc(); //쿨타임 측정이므로 update에서 돌아가야함. 
            TryFire(); //발사 입력 받는 부분은 update에서 굳이 돌려야할까? --> input 쓰는데? 생각해보기. 
            TryReload(); //재장전도 마찬가지 -> 키 눌렀을 때만 측정하면 되지 않을까? 
            TryFineSight(); //정조준 
        }
    }


    public void CancelReload() //리로드 동작 중지. 
    {
        if(isReload) //장전 중이면 
        {
            StopAllCoroutines(); //이거 올 코루틴 중지 막 돌려도 되는지는 모르겠네.
            isReload = false;
        }
    }

    //총으로 교체하고자 할 때 필요.. 호출은 WeaponManager에서 이루어짐
    public void GunChange(Gun gun) //이거 매개변수가 gun형인데 왜 딕셔너리 name이 받을 수 있는지? 
    {
        if (WeaponManager.currentWeapon!=null) //지금 손에 무기가 있으면 (current가 안 비어있다면)
        {
            WeaponManager.currentWeapon.gameObject.SetActive(false);              
        }

        currentGun = gun; 
        WeaponManager.currentWeapon=currentGun.GetComponent<Transform>();
        //WeaponManager.currentWeaponAnim=currentGun.anim; 애니메이션 교체 
        currentGun.transform.localPosition = Vector3.zero; //위치 초기화
        currentGun.gameObject.SetActive(true); //무기 켜주기. 

        isActivate = true; 

    }
    


    private void GunFireRateCalc()
    {
        if (currentFireRate > 0)
        {
            currentFireRate -= Time.deltaTime; // deltaTime만큼 지속적으로 감소 (총의 쿨타임)
        }

    }

    private void TryFire() //발사 입력을 받음. --> 이 부분 EquipController에서 관리하므로 인풋을 넣을 필요없음
    {
        if (currentFireRate <= 0 && !isReload) //쿨타임 <=0 이고 재장전 중이 아닐 때만 Fire 실행. 
        {
            Fire();
        }

    }

    private void Fire() //발사를 위한 과정 
    {
        if (!isReload)
        {
            if (currentGun.currentBulletCount > 0) //재장전 중이 아니면서 동시에 총알이 남아있으면 Shoot()실행. 
            {
                Shoot();
            }
            else
            {
                //원본에서는 여기서 Reload를 실행하지만 우리 게임은 Reload를 키를 눌러서 진행할 예정이다.
            }
        }
    }

    public void CancelFineSight() // 정조준 취소 함수
    {
        if (isFineSightMode)
        {
            FineSight();
        }
    }

    private void Shoot() //실제 발사되는 과정 
    {
        //muzzleEffect 생성 필요. + renderer 필요 시 추가 + 화염효과 추가 

        currentGun.currentBulletCount--; //총알 감소 
        currentFireRate = currentGun.fireRate; //연사 속도 재계산 ( deltaTime 빼줘서 0 되기전까지 다시 발사 중지)

        currentGun.muzzleFlash.Play(); //총 발사시에 이펙트 발생.
        PlaySE(currentGun.fire_Sound); //현재 총의 사운드 재생 

        //피격 처리
        Hit();

        //총기 반동 코루틴 실행
        StopAllCoroutines(); //반동 코루틴 멈추고
        StartCoroutine(RetroActionCoroutine());

    }


    private void Hit()
    {
        // 카메라 월드 좌표 (localPosition이 아니다. )
        // 플레이어에게 달려있는 1인칭 카메라로부터 RaYcAST를 쏴서 충돌 지점을 총알이 맞은 위치로 판정할 것. 
        // 1인칭 카메라 이기 때문에 사실상 화면 정중앙에 총을 쏘게 되기 때문에. 
        if (Physics.Raycast(theCam.transform.position, theCam.transform.forward, out hitInfo, currentGun.range))
        {
            poolContainer.GetBloodEffect(hitInfo.point, Quaternion.LookRotation(hitInfo.normal));

        }

        // 충돌 시 생성할 때 objectPool을 이용한다.
        // ParticleSystem hitEffect =GameManager.Pool.Get 이용!! --> pooled Object 상속해서 풀링해두기. 
    }

    private void TryReload() //리로드 또한 장비컨트롤러에서 실제 키와 연결되어 있으므로 인풋 제한 걸 필요없다.
    {
        if (!isReload)
        {
            CancelFineSight(); //정조준 상태 해제 후 리로드 시작. 
            StartCoroutine(ReloadCoroutine());

        }
    }

    private void PlaySE(AudioClip _clip) //발사 소리 재생 
    {
        audioSource.clip = _clip;
        audioSource.Play();
    }

    IEnumerator ReloadCoroutine() //재장전 코루틴. 
    {
        if (currentGun.currentBulletCount < currentGun.maxBulletCount) //max숫자는 변동없음. 
        {
            //현재 총탄의 갯수가 최대 총탄의 갯수보다 적으면 리로딩 진행
            isReload = true;
            //currentGun.anim.SetTrigger("Reload"); 여기서 애니메이션을 수행해야 된다면 재생.
            yield return new WaitForSeconds(currentGun.reloadTime); //재장전 애니메이션 동안 대기 

            currentGun.currentBulletCount = currentGun.maxBulletCount; //현재 총탄을 최대총탄 숫자로 맞춰줌.
            isReload = false;
        }

    }
    private void TryFineSight() //정조준 실행. 
    {
        /* if (Input.GetKeyDown("Fire2") && currentGun.gunType == Gun.GunType.SNIPER
             && !isReload) //스나이퍼 일 때만 정조준 진행. (??) ++ 장전 중이 아닐 때만 조준 가능하도록..
         {
             FineSight(); //나중에 equipCont로 옮겨줄 예정. 일단 더 생각해보기. 
         }*/
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


}
