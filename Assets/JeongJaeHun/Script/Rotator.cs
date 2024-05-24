using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float RotationSpeed = 50f;
    private void Update()
    {
        transform.Rotate(0f, RotationSpeed*Time.deltaTime, 0f);
    }
}
