using Cinemachine;
using Photon.Realtime;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] float mouseSensitivity;
    [SerializeField] float distance;
    [SerializeField] GameObject[] FPSIgnoreObject;
    [SerializeField] Transform target;
    [SerializeField] Transform FPSCameraRoot;
    [SerializeField] CinemachineVirtualCamera FPSCam;
    Vector2 inputDir;

    float yRotation;

    int FPSIgnoreLayerMask;
    public void Init()
    {
        FPSCam.Priority = 10;
        int ignoreLayer = LayerMask.NameToLayer("FPSIgnore"); //무시할 레이어 설정

        FPSIgnoreLayerMask = 1 << ignoreLayer; //쉬프트연산
        Camera.main.cullingMask = ~FPSIgnoreLayerMask; //컬링 레이어 설정

        if (FPSIgnoreObject == null)
            return;

        for (int i = 0; i < FPSIgnoreObject.Length; i++) //레이어 재설정
        {
            if (FPSIgnoreObject[i] == null)
                continue;
            //하위 객체들 또한 전부 레이어 재설정
            foreach (Transform childeGameObject in FPSIgnoreObject[i].GetComponentsInChildren<Transform>())
                childeGameObject.gameObject.layer = ignoreLayer;
        }
    }

    void Update()
    {
        transform.Rotate(Vector3.up, inputDir.x * SetValue()); //캐릭터 좌우 회전
        FPSCameraRoot.localRotation = Quaternion.Euler(yRotation, 0, 0); //카메라 상하 회전
    }

    float SetValue() //감도
    {
        target.position = Camera.main.transform.position + Camera.main.transform.forward * distance;// 바라볼 방향

        float value = mouseSensitivity * Time.deltaTime; //감도 계산
        yRotation -= inputDir.y * value; //감도만큼 상하 회전
        yRotation = Mathf.Clamp(yRotation, -85f, 45f); // 최대 각도 제한
        return value;
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
    }
    void OnLook(InputValue value)
    {
        inputDir = value.Get<Vector2>();
    }

    private void OnDestroy()
    {
        Destroy(FPSCameraRoot.gameObject); //컨트롤 파괴시 시네머신 카메라도 같이 파괴
    }

}
