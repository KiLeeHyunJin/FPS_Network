using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class SpyCamController : MonoBehaviourPun, ISkill
{
    public CinemachineVirtualCamera spyCamVirtualCamera;
    public GameObject spyCam;
    private bool isSpyCamActive = false;
    private bool isSpyCamPlaced = false;

    public void Activate()
    {
        
    }

    public void Deactivate()
    {

    }

    void Update()
    {
        // F 키를 누르면 스파이캠 활성화 및 비활성화
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleSpyCam();
        }

        // 스파이캠이 설치되지 않았고 활성화된 경우 스파이캠 설치
        if (!isSpyCamPlaced && isSpyCamActive)
        {
            PlaceSpyCam();
        }
    }

    void ToggleSpyCam()
    {
        isSpyCamActive = !isSpyCamActive;
        spyCamVirtualCamera.Priority = isSpyCamActive ? 10 : 0;
    }

    void PlaceSpyCam()
    {
        // 스파이캠을 마우스 클릭한 위치에 설치
        // if (input.GetMouseButtonDown(0))
        if (Input.GetKeyDown(KeyCode.G)) // 임시로
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                spyCam.transform.position = hit.point;
                spyCam.transform.rotation = Quaternion.LookRotation(hit.normal);
                isSpyCamPlaced = true;
                isSpyCamActive = false;
                spyCamVirtualCamera.Priority = 10;
            }
        }
    }
}
