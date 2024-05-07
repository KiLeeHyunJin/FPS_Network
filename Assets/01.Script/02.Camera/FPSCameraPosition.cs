using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCameraPosition : MonoBehaviour
{
    [SerializeField] Transform cameraRoot;
    CinemachineVirtualCamera cam;
    CameraController cameraController;
    private void Start()
    {
        cameraController = GetComponentInParent<CameraController>();
        cam = GetComponentInChildren<CinemachineVirtualCamera>();
        if(cam != null)
        {
            cam.transform.localPosition = Vector3.zero;
            cam.transform.localRotation = Quaternion.identity;
            cam.transform.SetParent(transform, true);
        }
    }

    void Update()
    {
        if(cameraController.POVType == CameraController.PointOfView.FPS)
        {
            transform.position = Vector3.Lerp(transform.position, cameraRoot.position, 0.5f);
            float front = -
                0.1f;
            float up = 0.1f;
            transform.position += transform.forward * 0.1f;
            transform.localPosition += new Vector3(0, up, front);
        }
    }
}
