using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCameraPosition : MonoBehaviour
{
    [SerializeField] Transform cameraRoot;
    CinemachineVirtualCamera cam;
    //[SerializeField] float front = -0.1f;
    //[SerializeField] float up = 0.1f;
    private void Start()
    {
        cam = GetComponentInChildren<CinemachineVirtualCamera>();
        if(cam != null)
        {
            cam.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            cam.transform.SetParent(transform, true);
        }
    }

    //void Update()
    //{
    //    transform.position = Vector3.Lerp(transform.position, cameraRoot.position, 0.5f);
    //    transform.localPosition += new Vector3(0, up, front);
    //}



}
