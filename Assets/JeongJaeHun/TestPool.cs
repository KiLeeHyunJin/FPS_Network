using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPool : MonoBehaviour
{

    [SerializeField]GameObject gg;
    private void Start()
    {
        gg = GameObject.FindGameObjectWithTag("KillLog");
        Debug.Log(gg.gameObject.name);
    }
    


}
