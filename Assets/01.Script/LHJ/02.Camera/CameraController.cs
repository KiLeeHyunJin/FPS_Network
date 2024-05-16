using Cinemachine;
using Photon.Realtime;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class CameraController //: MonoBehaviour
{
    Action<int> SetUserCharacterLayer;
    CinemachineVirtualCamera cam;
    CinemachineBasicMultiChannelPerlin noise;

    readonly FPSCameraPosition cameraRoot;
    readonly Controller owner;
    readonly Transform target;

    float distance = 10;
    float mouseSensitivity;
    float yRotation;

    float xPoskickUpValue;
    float yPoskickUpValue;

    float currentRecoil = 0f; // 현재 반동
    float recoilAmount = 1.5f; // 반동 증가량
    float maxRecoil = 0.4f; // 최대 반동

    float shakeIntensity = 1f; // 쉐이킹의 강도
    float shakeTimer = 0f; // 쉐이킹 지속 시간을 추적하는 타이머
    const float shakeDuration = 0.4f; // 쉐이킹 지속 시간

    bool isFire;

    public Vector2 InputDir { private get; set; }
    public float MouseSensitivity { set { mouseSensitivity = value; } }
    public int CameraPriority { set { cam.Priority = value; } }
    Action updateAction;
    public CameraController (Transform _aim, Controller _owner, FPSCameraPosition _cameraRoot)
    {
        cameraRoot = _cameraRoot;
        target = _aim;
        owner = _owner;
    }
    public void Init(Action<int> _layerMethod, CinemachineVirtualCamera _cam, float _mouseSensitivity)
    {
        mouseSensitivity = _mouseSensitivity;
        cam = _cam;
        _cam.Priority = 10;
        noise = _cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_AmplitudeGain = 0f;
        noise.m_FrequencyGain = 0f;

        int ignoreLayer = LayerMask.NameToLayer("FPSIgnore") ; //무시할 레이어 설정
        int iconignore1 = LayerMask.NameToLayer("MyMiniCam");
        int iconignore2 = LayerMask.NameToLayer("SearchEnemyCam");
        int ignoreFlag = (1 << ignoreLayer) | (1 << iconignore1) | (1<<iconignore2); //쉬프트연산

        SetUserCharacterLayer ??= _layerMethod;
        SetUserCharacterLayer?.Invoke(ignoreLayer);

        Camera.main.cullingMask = ~ignoreFlag; //컬링 레이어 설정
        Cursor.lockState = CursorLockMode.Locked;

        updateAction = UpdateMethod;
    }
    public void Update()
        => updateAction.Invoke();

    void UpdateMethod()
    {
        target.position = Camera.main.transform.position + Camera.main.transform.forward * distance;// 바라볼 방향
        float value = mouseSensitivity * Time.deltaTime; //감도 계산
        yRotation -= (InputDir.y * value); //감도만큼 상하 회전
        float xValue = InputDir.x * value;
        if (isFire)
        {
            xValue += xPoskickUpValue;
            yRotation -= yPoskickUpValue;
        }

        yRotation = Mathf.Clamp(yRotation, -80f, 55f); // 최대 각도 제한

        owner.transform.Rotate(Vector3.up, xValue); //캐릭터 좌우 회전
        cameraRoot.transform.localRotation = Quaternion.Euler(yRotation, 0, 0); //카메라 상하 회전
    }
    public void ChangeView(ViewType viewType)
    {
        if(owner.Mine)
        {
            Transform pos = viewType == ViewType.Stand ? cameraRoot.StandPos : cameraRoot.CrouchPos;
            cam.Follow = pos;
        }
    }

    public void Spectator(int value)
    {
        cam.Priority = value;
        cam.LookAt = target;
    }

    Coroutine shakeCo;
    public void GetCamShakeRoutine()
    {
        isFire = true;
        shakeTimer = shakeDuration;
        currentRecoil += recoilAmount * Time.deltaTime;
        currentRecoil = Mathf.Clamp(currentRecoil, 0f, maxRecoil);

        xPoskickUpValue = UnityEngine.Random.Range(-currentRecoil, currentRecoil);

        yPoskickUpValue += Mathf.Clamp((yPoskickUpValue + (Time.deltaTime)) * Time.deltaTime, 0, 3);

        noise.m_AmplitudeGain = shakeIntensity;
        noise.m_FrequencyGain = shakeIntensity;

        owner.StartCoroutined(Shake(),ref shakeCo);
    }

    IEnumerator Shake()
    {
        while (shakeTimer > 0f)
        {
            yield return new WaitForFixedUpdate();

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

    public enum ViewType
    { 
        Stand, Crouch, END
    }

}
