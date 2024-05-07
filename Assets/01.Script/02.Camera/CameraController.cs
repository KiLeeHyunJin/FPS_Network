using Cinemachine;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public enum PointOfView
    {
        TPS, FPS,
    }
    [Range(0.2f, 1)]
    [SerializeField] float crouchHeight;
    [SerializeField] float mouseSensitivity;
    [SerializeField] float distance;
    [SerializeField] GameObject[] FPSIgnoreObject;
    [SerializeField] Transform target;
    [SerializeField] Transform TPSCameraRoot;
    [SerializeField] Transform FPSCameraRoot;
    [SerializeField] CinemachineVirtualCamera TPSCam;
    [SerializeField] CinemachineVirtualCamera FPSCam;
    public PointOfView POVType { get; private set; }
    public bool Around
    {
        private get
        {
            return isAround;
        }
        set
        {
            if (isAround == true)
                ResetAroundRootRotation();
            isAround = value;
        }
    }
    Coroutine changePosCo = null;
    Action<float> View;

    Vector3 tpsOriginPos;
    Vector2 inputDir;

    float yRotation;

    bool isCrouch;
    bool isAround;
    int FPSIgnoreLayerMask;
    public void Start()
    {
        tpsOriginPos = TPSCameraRoot.localPosition;
        POVType = PointOfView.FPS;
        isCrouch = false;
        ChangeView();
        int ignoreLayer = LayerMask.NameToLayer("FPSIgnore");
        FPSIgnoreLayerMask = 1 << ignoreLayer;
        if (FPSIgnoreObject != null)
        {
            for (int i = 0; i < FPSIgnoreObject.Length; i++)
            {
                if (FPSIgnoreObject[i] != null)
                {
                    Transform[] objs = FPSIgnoreObject[i].GetComponentsInChildren<Transform>();
                    for (int j = 0; j < objs.Length; j++)
                    {
                        objs[j].gameObject.layer = ignoreLayer;
                    }
                }
            }
        }

    }
    public void ChangeView()
    {
        if (POVType == PointOfView.TPS)
        {
            View = FPSUpdate;
            POVType = PointOfView.FPS;
            TPSCam.Priority = 10;
            FPSCam.Priority = 50;
            Camera.main.cullingMask = ~FPSIgnoreLayerMask;
        }
        else
        {
            View = TPSUpdate;
            POVType = PointOfView.TPS;
            TPSCam.Priority = 50;
            FPSCam.Priority = 10;
            Camera.main.cullingMask = int.MaxValue;
        }
        CrouchState(isCrouch);
    }
    public void CrouchState(bool state)
    {
        isCrouch = state;
        if (POVType == PointOfView.TPS)
        {
            if (changePosCo != null)
                StopCoroutine(changePosCo);
            changePosCo = StartCoroutine(TPSCrouchChangePos(state));
        }
    }

    void Update()
    {
        View(SetValue());
    }
    void FPSUpdate(float value)
    {
        if (isAround)
            AroundOn(value, FPSCameraRoot);
        else
            FocusOn(value, FPSCameraRoot);
    }
    void TPSUpdate(float value)
    {
        if (isAround)
            AroundOn(value, TPSCameraRoot);
        else
            FocusOn(value, TPSCameraRoot);
    }

    void FocusOn(float value, Transform root)
    {
        transform.Rotate(Vector3.up, inputDir.x * value);
        root.localRotation = Quaternion.Euler(yRotation, 0, 0);

    }
    void AroundOn(float value, Transform root)
    {
        float currentX = GetXAngle(root.transform.localRotation.eulerAngles);
        if (POVType == PointOfView.FPS)
        {
            if (currentX > 120 && inputDir.x > 0)
            {
                inputDir.x = 0;
            }
            else if (currentX < -120 && inputDir.x < 0)
            {
                inputDir.x = 0;
            }
        }
        float xRot = currentX + (inputDir.x * value);
        root.localRotation = Quaternion.Euler(yRotation, xRot, 0);
    }


    void ResetAroundRootRotation()
    {
        yRotation = 0;
        inputDir = Vector2.zero;
        TPSCameraRoot.localRotation = Quaternion.identity;
    }
    float GetXAngle(Vector3 eulerAngle)
    {
        float angle = eulerAngle.y;
        if (angle < 0)
            angle += 360;
        else if (angle > 180)
            angle -= 360;
        return angle;
    }
    float SetValue()
    {
        SetTargetPos();
        float value = mouseSensitivity * Time.deltaTime;
        yRotation -= inputDir.y * value;
        if (POVType == PointOfView.TPS)
            yRotation = Mathf.Clamp(yRotation, -80f, 40f);
        else
            yRotation = Mathf.Clamp(yRotation, -85f, 45f);
        return value;
    }

    IEnumerator TPSCrouchChangePos(bool state)
    {
        float time = 0;
        Vector3 dest;
        Vector3 start = TPSCameraRoot.localPosition;
        float destDistance;
        float startDistance = TPSCam.m_Lens.FieldOfView; ;
        if (state)
        {
            dest = tpsOriginPos;
            dest.y -= crouchHeight;
            destDistance = 50;
        }
        else
        {
            dest = tpsOriginPos;
            destDistance = 55;
        }
        while (time < 1)
        {
            TPSCameraRoot.localPosition = Vector3.Lerp(start, dest, time);
            TPSCam.m_Lens.FieldOfView = Mathf.Lerp(startDistance, destDistance, time);
            time += Time.deltaTime;
            yield return null;
        }
        TPSCameraRoot.localPosition = dest;
        TPSCam.m_Lens.FieldOfView = destDistance;
    }
    void SetTargetPos()
    {
        target.position = Camera.main.transform.position + Camera.main.transform.forward * distance;
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
        Destroy(TPSCameraRoot.gameObject);
        Destroy(FPSCameraRoot.gameObject);
    }
}
