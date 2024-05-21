using System.Collections;
using UnityEngine;

public class Bomb : IKWeapon
{
    // 폭탄에 필요한 공통 조각

    // controller에서 불러오는 라인 렌더러 (수류탄 궤적 --> 던지기 전에 손에 들면 궤적이 생겨야함 )
    // 던지기 기능 (리지드바디 이용 포물선 운동 )
    // 던지기를 시작할 때 --> 리지드 바디 켜주고 + 충돌 켜주고 + 터지기 시간 준비 
    // 터지기 시간되면 폭발 --> 수류탄의 폭발 이펙트 섬광탄의 섬광 이펙트 
    // --> 추가적인 작업은 포스트프로세싱 불러오기 (스크립트로 존재 )
    // 던지는 쿨 타임 및 --> 숫자가 0 이 되면 총 들고 있는 상태로 전환해주기. ?? 아니면 그냥 헛모션 나오기 할까? 


    public enum BombType
    {
        GRENADE, FLASHBANG
    }

    public BombType bombType;



    [Header("HUD와 연계될 정보들")]
    [Tooltip("ui에 표현될 폭탄의 스프라이트")]
    public Sprite bombSprite;
    [Tooltip("폭탄의 이름")]
    public string bombName;

    [Tooltip("폭탄의 총 갯수")]
    public int fullBombNumber;
    [Tooltip("현재 남은 폭탄의 갯수")]
    public int currentBombNumber;
    /*[Tooltip("폭탄의 궤적을 나타낼 라인렌더러")]
    public LineRenderer lineRenderer;  // 그런데 이거 컨트롤러에서 하고 있는거 아닌가? */

    [Header("기타 이펙트 및 소리 등")]

    [Tooltip("수류탄 폭발 이펙트")]
    public ParticleSystem bomb_ParticleSystem;
    [Tooltip("섬광탄 폭발 이펙트")]
    public ParticleSystem flash_ParticleSystem;


    [Header("딜레이 및 폭발관련 변수들 ")]
    [Tooltip("발사 후 몇 초 후 폭발할지 딜레이")]
    public int delayTime;

    [Tooltip("폭탄 투척 딜레이 ")]
    public int throwDelay;

    [Tooltip("폭탄이 터지는 범위")]
    public float range;
    [Tooltip("데미지를 줄 타겟 레이어")]
    public LayerMask targetLayerMask;
    [Tooltip("수류탄의 데미지")]
    public int damage;

    [Tooltip("수류탄 폭발용 게임오브젝트")]
    public GameObject bombFX;
    [Tooltip("섬광탄 폭발 게임오브젝트")]
    public GameObject flashFX;

    private ProcessingController processing;

    [Tooltip("수류탄 폭발 소리 ")]
    public AudioClip bombSoundClip;
    [Tooltip("섬광탄 폭발 소리 ")]
    public AudioClip FlashSoundClip;


   new Rigidbody rigidbody;

    AudioSource audioSource;



    LineRenderer lineRenderer;


    private void OnEnable()
    {
        /*lineRenderer=GetComponent<LineRenderer>(); 
        lineRenderer.enabled=true;  */
    }
    private void OnDisable()
    {
        /*lineRenderer.enabled = false;*/
        rigidbody.isKinematic = true; // 키네마틱 on ( 투척시에 키네마틱 꺼줄 예정) 
    }
    protected override void Awake()
    {
        base.Awake();
    }

    public void Start()  //일단 기본적으로 시작할 때 rigidbody kinematic 실시해줘야함. 
    {
        rigidbody = GetComponent<Rigidbody>();

        audioSource = GetComponent<AudioSource>();  //폭탄에 붙어 있는데 어째서 ??

        audioSource.loop = false;  //  loop 꺼주기.
        audioSource.playOnAwake = false; //시작하자마자 소리 키기 꺼주기. 

        damage = 50;
    }


    // 인보크 등 코루틴이던 다른 곳에서 실행 시켜줄? 아니면 여기서 실행할 수도 있고 생각해보자. 
    public void CountDownBomb(GameObject instanceBomb) //폭탄 투척시 카운트 다운임. --?컨트롤러에서 투척 실행 시 이 함수를 부르자.
    {
        if (instanceBomb == null)
            return;

        Bomb ins = instanceBomb.GetComponent<Bomb>();


        switch (ins.bombType)  //자신의 타입에 따라 다른 함수 실행. 
        {
            case BombType.GRENADE:
                ins.StartCoroutine(Grenade(instanceBomb, ins));

                break;
            case BombType.FLASHBANG:
                ins.StartCoroutine(FlashBang(instanceBomb, ins));

                break;

        }
    }

    Collider[] colliders = new Collider[20];

    private void BombFindTarget()
    {
        
        int size = Physics.OverlapSphereNonAlloc(transform.position, range, colliders, targetLayerMask);

        for (int i = 0; i < size; i++) //한 번 player가 체크되면 데미지 주고 빠져나가야함. 
        {
            Controller controller = colliders[i].gameObject.GetComponent<Controller>();

            if (controller != null) //targetLayer가 controller를 가지고 있다면. 데미지 주기. 
            {
                controller.Damage(damage); //데미지 함수 내부에서 쉴드체크 해주고 있음. !
                controller.GetComponent<ProcessingController>()?.HitEffect();

            }
        }
    }

    private void FlashFindTarget()
    {
        //범위내에 사람이 존재 한다면 processing 효과 진행.

        int size = Physics.OverlapSphereNonAlloc(transform.position, range, colliders, targetLayerMask);

        for (int i = 0; i < size; i++)
        {
            Controller controller = colliders[i].gameObject.GetComponent<Controller>();
            if (controller != null)
            {
                controller.GetComponent<ProcessingController>()?.FlashEffect();
            }
        }
    }
    private IEnumerator Grenade(GameObject instanceBomb, Bomb bomb) //수류탄 터질 시 발생할 함수
    {

        yield return new WaitForSeconds(delayTime); //지금 이 뒤로 진행이 안되는데?
        BombFindTarget();


        GameObject bombFxIns = Instantiate(bombFX, instanceBomb.transform.position, Quaternion.identity);
        bomb.audioSource.PlayOneShot(bombSoundClip, 1.0f); //특정 클립 한 번만 재생 --> 매개변수 2번째는 소리크기 조절.
        yield return new WaitForSeconds(2f);

        Destroy(instanceBomb.gameObject);
        Destroy(bombFxIns);


    }

    private IEnumerator FlashBang(GameObject instanceBomb, Bomb bomb) // 섬광탄 터질 시 발생할 함수 
    {
        yield return new WaitForSeconds(delayTime);

        FlashFindTarget();

        GameObject flashFxINS = Instantiate(flashFX, instanceBomb.transform.position, Quaternion.identity);

        bomb.audioSource.PlayOneShot(FlashSoundClip, 1.0f); //섬광탄 소리 
        yield return new WaitForSeconds(2f);
        Destroy(instanceBomb.gameObject);
        Destroy(flashFxINS);

    }










}