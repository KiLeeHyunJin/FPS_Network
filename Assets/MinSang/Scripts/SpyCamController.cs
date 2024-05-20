using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;
using Unity.IO.LowLevel.Unsafe;

public class SpyCamController : MonoBehaviourPun, ISkill
{
    [SerializeField] public CinemachineVirtualCamera spyCamVirtualCamera;
    [SerializeField] public GameObject spyCamPrefab;
    [SerializeField] private bool isSpyCamActive = false;
    [SerializeField] private bool isSpyCamPlaced = false;

    private GameObject currentSpyCam;

    public void Activate()
    {
        if (currentSpyCam != null)
        {
            spyCamVirtualCamera.Priority = 20;
            isSpyCamActive = true;
        }
    }

    public void Deactivate()
    {
        if (currentSpyCam != null)
        {
            spyCamVirtualCamera.Priority = 0;
            isSpyCamActive = false;
        }
    }
    public void Start()
    {
        spyCamVirtualCamera.Priority = 0;
    }
    public void Update()
    {
        // F 키를 누르면 스파이캠 활성화 및 비활성화
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("스파이캠 활성화/비활성화");
            ToggleSpyCam();
        }

        // 스파이캠이 설치되지 않았고 활성화된 경우 스파이캠 설치
        if (!isSpyCamPlaced && isSpyCamActive)
        {
            StartCoroutine(PlaceSpyCam());
        }

        if (isSpyCamPlaced && isSpyCamActive)
        {
            RotateSpyCam();
        }
    }

    void ToggleSpyCam()
    {
        isSpyCamActive = !isSpyCamActive;
        spyCamVirtualCamera.Priority = isSpyCamActive ? 10 : 0;

        if (isSpyCamActive && currentSpyCam == null)
        {
            Debug.Log("스파이캠 활성화");
            StartCoroutine(PlaceSpyCam());
        }
        else if (isSpyCamActive && currentSpyCam != null)
        {
            Debug.Log("스파이캠 비활성화");
            Deactivate();
        }
    }

    IEnumerator PlaceSpyCam()
    {
        while (!isSpyCamPlaced && isSpyCamActive)
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (IsPlacementValid(hit.point))
                    {
                        photonView.RPC("RPC_PlacedSpyCam", RpcTarget.AllBuffered, hit.point, hit.normal);
                        isSpyCamActive = false;
                        isSpyCamPlaced = false;
                        spyCamVirtualCamera.Priority = 0;
                    }
                }
            }
            yield return null;
        }
    }
    [PunRPC]
    void RPC_PlacedSpyCam(Vector3 position, Vector3 normal)
    {
        currentSpyCam = Instantiate(spyCamPrefab, position, Quaternion.LookRotation(normal));
        spyCamVirtualCamera.Follow = currentSpyCam.transform;
        spyCamVirtualCamera.LookAt = currentSpyCam.transform;
        isSpyCamPlaced = true;
    }

    bool IsPlacementValid(Vector3 position)
    {
        if (Physics.CheckSphere(position, 0.5f))
        {
            return true; // 충돌하는 물체가 있으면 설치, false로 바꾸면 설치 불가
        }

        // 지면 검사: 스파이캠이 특정 지면이나 벽에만 설치되도록 제한
        // 레이캐스트를 아래로 쏴서 지면을 확인
        RaycastHit groundHit;
        Debug.DrawRay(position, Vector3.down * 1.0f, Color.red, 10f);
        if (!Physics.Raycast(position, Vector3.down, out groundHit, 1.0f))
        {
            return false; // 지면이 없으면 설치 불가
        }

        if (groundHit.collider.tag != "Ground")
        {
            return false; // 유효한 지면이 아니면 설치 불가
        }

        // 범위 제한: 플레이어와 너무 멀거나 가까운 위치에 설치되지 않음
        float maxDistance = 10.0f; // 최대 거리
        float minDistance = 1.0f;  // 최소 거리
        if (Vector3.Distance(transform.position, position) > maxDistance || Vector3.Distance(transform.position, position) < minDistance)
        {
            return false; // 설치 위치가 유효한 범위를 벗어나면 설치 불가
        }

        return true;
    }

    void RotateSpyCam()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            currentSpyCam.transform.Rotate(Vector3.up, -1f);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            currentSpyCam.transform.Rotate(Vector3.up, 1f);
        }
    }
}
