using Photon.Pun;
using System.Collections;
using UnityEngine;
using static Bomb;

public class BombController : MonoBehaviourPun, Iattackable
{
    private Camera mainCamera; //메인 카메라

    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] float throwPower; //수류탄 투척력

    [SerializeField] private Bomb currentBomb; //현재 들고 있는 폭탄 --> 수류탄 프리팹이나 마찬가지 아니냐?
    [SerializeField] Controller controller;
    //[SerializeField] private Rigidbody bombRigidbody; // 자식의 폭탄의 리지드바디 가져와서 kinematic 으로 해주고 던지면 해제. 

    private GameObject instanceBomb; //실제 날리는게 아니라 생성해서 날려줄 폭탄. 

    private Slot slot;
    new AudioSource audio;

    // throw 발동시에.--> 딜레이 주기 (막 던지기 불가능하도록 애니메이션과 맞춤)
    // 무기 전환 시에 무기 투척 불가능하도록 해야함. 
    void Awake()
    {
        audio = GetComponent<AudioSource>();
        audio.playOnAwake = false;
        audio.loop = false;

    }
    public Bomb CurrentBomb 
    { 
        get 
        { 
            if(currentBomb == null)
            {
                int numOfChild = this.transform.childCount;
                for (int i = 0; i < numOfChild; i++)
                {
                    if (transform.GetChild(i).gameObject.activeSelf == true) //이거 다 쓰고나면 true가 아닌데 어째서 켜지지?
                    {
                        currentBomb = transform.GetChild(i).GetComponent<Bomb>();
                        break;
                    }
                }
            }
            return currentBomb;
        } 
    }

    // 붐이 지금 준비가 되어 있는 상태라면... 궤적을 보여줘야함. 

    private void OnEnable() //기본적으로 꺼져있다가 켜질 거기 때문에 궤적 여기서 그려주기.
    {
        // currentBomb을 찾으려면?? --> 자기 자신의 자식들 중에서 (active 되어 있는 것을 찾아서 넣어주자.)
        // 이 부분도 자식의 currentbomb에서 리지드바디 가져오는 부분인데 어떻게 할지 생각좀 해보자. 
        mainCamera = Camera.main;
        if(TryGetComponent<LineRenderer>(out lineRenderer))
        {
            //ShowTrajectory(); //폭탄을 들고 있는 상태가되면 궤적을 보여줌. 
            updateCo = StartCoroutine(UpdateLineLender());
        }
    }
    Coroutine updateCo;
    IEnumerator UpdateLineLender()
    {
        lineRenderer.enabled = true; // 렌더러 켜주기. 
        while (true)
        {
            ShowTrajectory();
            yield return new WaitForSeconds(0.1f);
        }
    }

    public bool Attack()
    {
        return Throw();
    }

    private void OnDisable()
    {
        if (updateCo != null)
            StopCoroutine(updateCo);

        if (lineRenderer.enabled)
        {
            lineRenderer.enabled = false;
        }
    }

    private void Start()
    {
        slot = GetComponent<Slot>(); //자신이 slot을 가지고 있으므로. 
        controller ??= GetComponentInParent<Controller>();
    }

    private bool Throw() //투척 함수 
    {
        if (currentBomb.currentBombNumber <= 0)
        {
            return false; //발사불가능
        }

        // 이게 손에 들어가 있는 상황에서는 transform을 어떻게 잡아줘야 될지 고민되네
        // local 인지 전역 position인지.. 
        Vector3 forward = mainCamera.transform.forward; //카메라의 전방으로 라인을 그릴 거기 때문에 
        Vector3 startVelocity = throwPower * forward; // 전방으로 던지는 힘. 
        Vector3 startPosition = transform.position;
        startPosition.y += startPosition.y + 1f;

        photonView.RPC("ThrowBomb", RpcTarget.MasterClient, startPosition, startVelocity, (int)currentBomb.bombType);
        photonView.RPC("EffectVoice", RpcTarget.MasterClient, (int)currentBomb.bombType);

        // 실제 인벤토리의 아이템을 투척하는 것이 아니라 인스턴스를 생성하고 그것을 투척함. 

        //Vector3 startPosition = transform.localPosition;  --> 이거 실험해보자. 

        //아무튼 실제로는 던져도 실물을 던지는게 아니라 복사본? 약간 그런걸 던지면됨. 
        //rigidGrenade.isKinematic = false; //키네마틱 해제. 

        //currentBomb.CountDownBomb(instanceBomb);  //현재 폭탄의 실제 폭발 함수 실행. -->내부에서 case에 따라 효과전환해줌. 

        currentBomb.currentBombNumber--;

        if (currentBomb.currentBombNumber <= 0) //이거 폭탄 컨트롤러가 아니라 폭탄 자체를 꺼주면 아마 못찾을거야 체크를 그렇게하니까
        {
            currentBomb.gameObject.SetActive(false); //그런데 찾아지는 살짝의 버그있음. 
            slot.notHaving = true;
            controller.CallDefaultPose();
            return false;
        }
        return true;
    }
    [PunRPC]
    void EffectVoice(int num)
    {
        BombType bombType = (BombType)num;
        
        audio.clip = bombType switch
        {
            BombType.GRENADE => currentBomb.bombVoiceClip,
            BombType.FLASHBANG => currentBomb.flashBangVoiceClip,
            _ => null
        };
        if (audio.clip != null)
            audio.Play();
    }

    [PunRPC]
    void ThrowBomb(Vector3 position, Vector3 velocity, int bombType)
    {
        instanceBomb = PhotonNetwork.Instantiate(currentBomb.name, position, Quaternion.identity);
        instanceBomb.GetComponent<Bomb>().Direction(velocity, bombType);
    }

    public bool Reload()
    {
        return false;
    }

    private void ShowTrajectory() //궤도를 보여줌 (라인렌더러)
    {
        // 궤적 표시 관련 변수 초기화
        int linePoint = 1;
        float timePoint = 0.1f;

        //라인 렌더러 점 개수 설정
        lineRenderer.positionCount = Mathf.CeilToInt(linePoint / timePoint) + 1;

        // 초기 위치 및 방향 설정
        Vector3 forward = mainCamera.transform.forward;
        Vector3 startVelocity = throwPower * forward;
        Vector3 startPosition = transform.position; //홀더 위치? --> 홀더도 자식이니까 실제 홀더면 local인가?       
        startPosition.y += startPosition.y + 1f;

        //초기 위치 설정
        lineRenderer.SetPosition(0, startPosition);

        for (int i = 0; i < lineRenderer.positionCount; i++) // 1/0.1 이므로 10개의 선으로 이루어져 곡선을 그림
        {
            float time = i * timePoint;

            // 시간에 따른 궤적 위치 계산 
            Vector3 point = startPosition + (time * startVelocity);
            point.y = startPosition.y + startVelocity.y * time + (Physics.gravity.y / 2f * time * time);

            // 라인 렌더러에 궤적 위치 설정
            lineRenderer.SetPosition(i, point);
        }
    }


}
