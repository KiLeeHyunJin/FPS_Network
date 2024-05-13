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

    Transform character;
    Transform target;

    float distance = 10;
    float mouseSensitivity;
    float yRotation;

    float shakeIntensity = 0.7f; // 쉐이킹의 강도
    const float shakeDuration = 0.4f; // 쉐이킹 지속 시간
    float shakeTimer = 0f; // 쉐이킹 지속 시간을 추적하는 타이머

    int FPSIgnoreLayerMask;

    public Vector2 inputDir { private get; set; }

    public CameraController (Transform _aim, Transform _owner, FPSCameraPosition _cameraRoot, float _mouseSensitivity)
    {
        target = _aim;
        character = _owner;
        cameraRoot = _cameraRoot;
        mouseSensitivity = _mouseSensitivity;
    }
    public void Init(CinemachineVirtualCamera _cam, Action<int> _layerMethod)
    {
        _cam.Priority = 10;
        int ignoreLayer = LayerMask.NameToLayer("FPSIgnore"); //무시할 레이어 설정
        noise = _cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        //ChangeLayer(ignoreLayer);
        FPSIgnoreLayerMask = 1 << ignoreLayer; //쉬프트연산
        Camera.main.cullingMask = ~FPSIgnoreLayerMask; //컬링 레이어 설정
        Cursor.lockState = CursorLockMode.Locked;
        SetUserCharacterLayer = _layerMethod;
        SetUserCharacterLayer?.Invoke(ignoreLayer);
        noise.m_AmplitudeGain = 0f;
        noise.m_FrequencyGain = 0f;
    }

    public void Update()
    {
        character.transform.Rotate(Vector3.up, inputDir.x * SetValue()); //캐릭터 좌우 회전
        cameraRoot.transform.localRotation = Quaternion.Euler(yRotation, 0, 0); //카메라 상하 회전
    }

    float SetValue() //감도
    {
        target.position = Camera.main.transform.position + Camera.main.transform.forward * distance;// 바라볼 방향

        float value = mouseSensitivity * Time.deltaTime; //감도 계산
        yRotation -= inputDir.y * value; //감도만큼 상하 회전
        yRotation = Mathf.Clamp(yRotation, -85f, 45f); // 최대 각도 제한
        return value;
    }
    public IEnumerator GetCamShakeRoutine()
        => Shake();
    IEnumerator Shake()
    {
        shakeTimer = shakeDuration;
        Debug.Log("Shake!");
        while (shakeTimer > 0f)
        {
            noise.m_AmplitudeGain = shakeIntensity;
            noise.m_FrequencyGain = shakeIntensity; // X축과 Y축의 흔들림을 동일하게 설정
            shakeTimer -= Time.deltaTime;
            yield return null;
        }
        noise.m_AmplitudeGain = 0f;
        noise.m_FrequencyGain = 0f;
    }


}
