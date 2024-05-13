using Cinemachine;
using Photon.Realtime;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController //: MonoBehaviour
{
    Action<int> SetUserCharacterLayer;

    CinemachineBasicMultiChannelPerlin noise;
    FPSCameraPosition cameraRoot;
    Controller owner;
    Transform target;

    float distance = 10;
    float mouseSensitivity;
    float yRotation;

    float xPoskickUpValue;
    float yPoskickUpValue;

    float currentRecoil = 0f; // 현재 반동
    float recoilAmount = 0.1f; // 반동 증가량
    float maxRecoil = 0.4f; // 최대 반동

    float shakeIntensity = 1f; // 쉐이킹의 강도
    float shakeTimer = 0f; // 쉐이킹 지속 시간을 추적하는 타이머
    const float shakeDuration = 0.4f; // 쉐이킹 지속 시간

    int FPSIgnoreLayerMask;
    bool isFire;

    public Vector2 inputDir { private get; set; }

    public CameraController (Transform _aim, Controller _owner, FPSCameraPosition _cameraRoot, float _mouseSensitivity)
    {
        target = _aim;
        owner = _owner;
        cameraRoot = _cameraRoot;
        mouseSensitivity = _mouseSensitivity;
    }
    public void Init(CinemachineVirtualCamera _cam, Action<int> _layerMethod)
    {
        _cam.Priority = 10;
        noise = _cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        noise.m_AmplitudeGain = 0f;
        noise.m_FrequencyGain = 0f;

        int ignoreLayer = LayerMask.NameToLayer("FPSIgnore"); //무시할 레이어 설정
        FPSIgnoreLayerMask = 1 << ignoreLayer; //쉬프트연산

        SetUserCharacterLayer = _layerMethod;
        SetUserCharacterLayer?.Invoke(ignoreLayer);

        Camera.main.cullingMask = ~FPSIgnoreLayerMask; //컬링 레이어 설정
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Update()
    {
        target.position = Camera.main.transform.position + Camera.main.transform.forward * distance;// 바라볼 방향
        float value = mouseSensitivity * Time.deltaTime; //감도 계산
        yRotation -= (inputDir.y * value); //감도만큼 상하 회전
        float xValue = inputDir.x * value;
        if (isFire)
        {
            xValue += xPoskickUpValue;
            yRotation -= yPoskickUpValue;
        }

        yRotation = Mathf.Clamp(yRotation, -85f, 45f); // 최대 각도 제한

        owner.transform.Rotate(Vector3.up, xValue); //캐릭터 좌우 회전
        cameraRoot.transform.localRotation = Quaternion.Euler(yRotation, 0, 0); //카메라 상하 회전
    }
    Coroutine shakeCo;
    public void GetCamShakeRoutine()
    {
        owner.StartCoroutine(Shake(),ref shakeCo);
    }
    IEnumerator Shake()
    {
        isFire = true;
        shakeTimer = shakeDuration;
        currentRecoil += recoilAmount * 0.5f;
        currentRecoil = Mathf.Clamp(currentRecoil, 0f, maxRecoil);
        xPoskickUpValue = UnityEngine.Random.Range(-currentRecoil, currentRecoil);

        yPoskickUpValue += (yPoskickUpValue + (Time.deltaTime)) * Time.deltaTime;
        while (shakeTimer > 0f)
        {
            noise.m_AmplitudeGain = shakeIntensity;
            noise.m_FrequencyGain = shakeIntensity; // X축과 Y축의 흔들림을 동일하게 설정
            yield return null;

            isFire = false;
            float deltaTime = Time.deltaTime;

            shakeTimer -= deltaTime;
            currentRecoil -= deltaTime;

            if (currentRecoil < 0)
                currentRecoil = 0;
            if (yPoskickUpValue < 0)
                yPoskickUpValue = 0;
        }

        xPoskickUpValue = 0;
        yPoskickUpValue = 0;

        noise.m_AmplitudeGain = 0f;
        noise.m_FrequencyGain = 0f;
    }


}
