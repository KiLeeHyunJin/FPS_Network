using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyPos : MonoBehaviour
{
    [SerializeField] bool x, y, z;
    [SerializeField] Transform target;

    private void Update()
    {
        if (!target)
            return;
        transform.position = new Vector3(
            (x ? target.position.x : transform.position.x),
            (y ? target.position.y : transform.position.y),
            (z ? target.position.z : transform.position.z));
            
    }


}
