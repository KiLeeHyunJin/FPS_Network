using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AttackCheck : MonoBehaviour
{
    float sleepTime; //슬립 타임? 

    Transform aim; //자신의 총구 Aim 

    Camera mainCamera;

    private void Start()
    {
        
    }



    public void SetAimTransform(Transform _aim)
    {
        aim = _aim; //각각의 총들에서 CurrentGun의 Aim을 가져와서 저장 
    }

    public Vector3 AttackVectorCheck()
    {
        if (sleepTime != 0)
            sleepTime = 0;

        if (aim == null)
            return Vector3.zero;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hitInfo, Mathf.Infinity))
        {
            if (hitInfo.collider != null)
            {
                return (hitInfo.transform.position - aim.position).normalized;

                //if (Physics.Raycast(aim.position, direction, out hitInfo, Mathf.Infinity))
                //    return hitInfo.collider.GetComponent<Controller>();
            }
        }
        return Vector3.zero;

    }



}
