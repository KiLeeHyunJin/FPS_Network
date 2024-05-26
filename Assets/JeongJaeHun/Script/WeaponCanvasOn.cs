using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCanvasOn : MonoBehaviour
{
    private void OnEnable() // player 보다 이 awake가 먼저 발동하겠지? 
    {
        WeaponHUDAwake();
    }

    public GunHUD GunHUD;
    public CloseWeaponHUD CloseWeaponhud;
    public BombHUD BombHUD;

    public void WeaponHUDAwake()
    {
        int numChild = transform.childCount;

        for (int i = 0; i < numChild; i++)
        {
            Debug.Log(transform.GetChild(i).name);

            if (transform.GetChild(i).gameObject.activeSelf == false)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }

}
