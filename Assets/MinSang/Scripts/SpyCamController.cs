using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class SpyCamController : Skill, IPunObservable
{
    [SerializeField] private CinemachineVirtualCamera spyCamVirtualCamera;
    [SerializeField] private GameObject spyCamPrefab;

    private bool isSpyCamActive = false; // 스파이 캠이 활성화되었는지 여부
    private bool isSpyCamPlaced = false; // 스파이 캠이 배치되었는지 여부

    private GameObject currentSpyCam;
    private List<GameObject> spyCams = new List<GameObject>();
    private const int maxSpyCams = 3; // 최대 스파이 캠 수
    PhotonView pv;
    public override void SkillOn()
    {
        Debug.Log(SkillName + "SkillOn");
    }

    public override void SkillOff()
    {
        Debug.Log(SkillName + "SkillOff");
    }

    // 초기 설정 함수
    private void Start()
    {
        // PhotonView 컴포넌트 가져오기
        pv = GetComponent<PhotonView>();
        if (pv == null)
        {
            Debug.LogError("PhotonView component is missing on the GameObject");
        }
        else
        {
            // 동적으로 SpyCamController를 관찰 중인 컴포넌트 리스트에 추가
            if (!pv.ObservedComponents.Contains(this))
            {
                pv.ObservedComponents.Add(this);
            }
        }

        // 스파이 캠 프리팹이 할당되었는지 확인
        if (spyCamPrefab == null)
        {
            Debug.LogError("spyCamPrefab is not assigned in the Inspector");
        }

        // spyCamVirtualCameraPrefab 할당 여부 확인
        if (spyCamVirtualCamera == null)
        {
            Debug.LogError("spyCamVirtualCameraPrefab is not assigned in the Inspector");
        }
        else
        {
            // 프리팹 인스턴스화
            CinemachineVirtualCamera instantiatedCamera = Instantiate(spyCamVirtualCamera,transform.position,Quaternion.identity);
            spyCamVirtualCamera = instantiatedCamera;
        }

    }

    // 스파이 캠을 활성화하는 함수
    public void Activate()
    {
        if (currentSpyCam != null)
        {
            SetSpyCamView(true);
        }
        else
        {
            Debug.LogWarning("No spy camera to activate");
        }
    }

    // 스파이 캠을 비활성화하는 함수
    public void Deactivate()
    {
        if (currentSpyCam != null)
        {
            SetSpyCamView(false);
        }
    }

    // 스파이 캠의 뷰를 설정하는 함수
    private void SetSpyCamView(bool isActive)
    {
        spyCamVirtualCamera.Priority = isActive ? 120 : -10; // 활성화 시 우선순위를 높임
        Debug.Log(spyCamVirtualCamera.Priority);
        // spyCamVirtualCamera.Follow = isActive ? currentSpyCam.transform : null;
        // spyCamVirtualCamera.LookAt = isActive ? currentSpyCam.transform : null; // 활성화 시 스파이 캠을 바라봄
        spyCamVirtualCamera.gameObject.SetActive(isActive); // 가상 카메라 활성화/비활성화
        isSpyCamActive = isActive;
        spyCamVirtualCamera.gameObject.SetActive(isSpyCamActive);
    }

    // 매 프레임 호출되는 업데이트 함수
    void Update()
    {
        if (pv == null) return;

        // F 키를 누르면 스파이 캠을 활성화 또는 비활성화
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("스파이캠 활성화 / 비활성화");
            ToggleSpyCam(); // 스파이 캠 활성화/비활성화 토글
        }

        // 스파이 캠이 활성화되어 있고 배치된 상태에서 회전 처리
        if (isSpyCamActive && isSpyCamPlaced && currentSpyCam != null)
        {
            RotateSpyCam(); // 스파이 캠 회전
        }
    }

    // 스파이 캠의 활성화/비활성화를 토글하는 함수
    public void ToggleSpyCam()
    {
        if (!isSpyCamActive)
        {
            if (currentSpyCam == null)
            {
                StartCoroutine(PlaceSpyCam()); // 스파이 캠 배치 코루틴 시작
            }
            else
            {
                Activate();
            }
        }
        else
        {
            Deactivate();
        }
    }
    IEnumerator PlaceSpyCam()
    {
        isSpyCamActive = true; // 스파이 캠 활성화 상태 설정
        while (!isSpyCamPlaced && isSpyCamActive)
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                TryPlaceSpyCam(); // 스파이 캠 배치 시도
            }
            yield return null; // 다음 프레임까지 대기
        }
    }

    public void TryPlaceSpyCam()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // 마우스 위치에서 Ray 발사
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log("Raycast hit: " + hit.point); // Raycast 충돌 지점 출력
            if (IsPlacementValid(hit.point))
            {
                Debug.Log("Valid placement position: " + hit.point); // 유효한 위치 출력
                pv.RPC("RPC_PlacedSpyCam", RpcTarget.AllBuffered, hit.point, hit.normal); // RPC 호출로 스파이 캠 배치
                isSpyCamActive = false; // 스파이 캠 활성화 상태 설정
                isSpyCamPlaced = true; // 스파이 캠 배치 상태 설정
                spyCamVirtualCamera.Priority = 0; // 가상 카메라 우선순위 낮춤
                Debug.Log(spyCamVirtualCamera.Priority);
                StopCoroutine(PlaceSpyCam()); // 코루틴 종료
            }
            else
            {
                Debug.LogWarning("Invalid placement position: " + hit.point); // 유효하지 않은 위치 출력
            }
        }
    }

    // RPC 메소드로 스파이 캠을 배치하는 함수
    [PunRPC]
    public void RPC_PlacedSpyCam(Vector3 position, Vector3 normal)
    {
        Debug.Log("Placing spy cam at position: " + position); // 스파이캠 배치 위치 출력
                                                               // 최대 스파이 캠 수를 초과하면 가장 오래된 스파이 캠을 제거
        if (spyCams.Count >= maxSpyCams)
        {
            Destroy(spyCams[0]); // 가장 오래된 스파이 캠 제거
            spyCams.RemoveAt(0); // 리스트에서 제거
        }

        // 새로운 스파이 캠을 생성하고 리스트에 추가
        // PhotonNetwork.Instantiate 사용
        // 프리팹 이름이 정확히 일치해야 함 (프리팹이 Resources 폴더에 있어야 함)
        Debug.Log("Instantiating spy cam prefab: " + spyCamPrefab.name);
        currentSpyCam = PhotonNetwork.Instantiate("spyCamPrefab", position, Quaternion.LookRotation(normal));
        if (currentSpyCam == null)
        {
            Debug.LogError("Failed to instantiate spyCamPrefab");
            return;
        }
        else
        {
            Debug.Log("Spy cam instantiated successfully");
        }

        spyCams.Add(currentSpyCam); // 리스트에 추가
        // spyCamVirtualCamera.Follow = currentSpyCam.transform;
        Debug.Log(spyCamVirtualCamera.Follow+"4455");
        // spyCamVirtualCamera.LookAt = currentSpyCam.transform;
        isSpyCamPlaced = true;

        // 스파이 캠의 뷰 설정
        spyCamVirtualCamera.m_Lens.FieldOfView = 60;
        Debug.Log(spyCamVirtualCamera);
        CinemachineTransposer aa = spyCamVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        if(aa!=null)
        {
            aa.m_FollowOffset = new Vector3(0, 0, 5);
        }

    }

    public bool IsPlacementValid(Vector3 position)
    {
        // 충돌 영역 검사
        Collider[] colliders = Physics.OverlapSphere(position, 5f);

        Debug.Log("설치 함수 진입");
        // 지면 검사
        if (!Physics.Raycast(position, Vector3.down, out RaycastHit groundHit, 2.0f))
        {
            return true;
        }

        if (groundHit.collider.tag != "Ground")
        {
            return false;
        }

        // 거리 제한 검사
        float maxDistance = 10.0f;
        float minDistance = 1.0f;
        float distance = Vector3.Distance(transform.position, position);
        if (distance > maxDistance || distance < minDistance)
        {
            Debug.LogWarning($"Placement position is out of valid range: {distance}");
            return false;
        }

        return true;
    }

    // 스파이 캠을 회전시키는 함수
    void RotateSpyCam()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            currentSpyCam.transform.Rotate(Vector3.up, -3f); // 왼쪽 화살표 키를 누르면 좌회전
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            currentSpyCam.transform.Rotate(Vector3.up, 3f); // 오른쪽 화살표 키를 누르면 우회전
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }

    public void SwitchToSpyCam()
    {
        if (currentSpyCam != null)
        {
            Transform spyCamTransform = currentSpyCam.transform;
            spyCamVirtualCamera.transform.position = spyCamTransform.position + spyCamTransform.forward * 2; // 스파이 캠 기준 앞쪽으로 카메라 위치 설정
            spyCamVirtualCamera.transform.rotation = spyCamTransform.rotation;
            SetSpyCamView(true);
        }
        else
        {
            Debug.LogWarning("No spy cam is currently placed.");
        }
    }
}