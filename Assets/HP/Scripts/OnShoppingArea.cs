using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnShoppingArea : MonoBehaviour
{
    [SerializeField]LayerMask playerLayer;
    [SerializeField] GameObject ShopCanvasPrefab;
    private void OnEnable()
    {
        playerLayer = LayerMask.GetMask("Player");
    }
    private void OnDisable()
    {
        //Close ShopList
    }


    private void OnTriggerEnter(Collider other)
    {
        if (playerLayer.ContainCheck(other.gameObject.layer)&& PhotonNetwork.CurrentRoom.GetProperty<bool>(DefinePropertyKey.SHOPPINGTIME))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Log("ShopOpen");
            ShopCanvasPrefab.SetActive(true);
            Manager.Game.onShop = true;
            //Open ShopList  
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (playerLayer.ContainCheck(other.gameObject.layer))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            ShopCanvasPrefab.SetActive(false);
            Manager.Game.onShop = false;
            Debug.Log("ShopClose");
            //Close ShopList
        }
    }

}
