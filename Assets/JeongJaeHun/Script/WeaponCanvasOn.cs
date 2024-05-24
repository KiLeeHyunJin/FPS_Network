using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCanvasOn : MonoBehaviour
{
    private void Awake()
    {
        int numChild = transform.childCount;

        for(int i= 0; i < numChild; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf==false)
            {
                transform.GetChild(i).gameObject.SetActive(true); 
            }
        }
    }

}
