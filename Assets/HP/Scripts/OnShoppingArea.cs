using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnShoppingArea : MonoBehaviour
{
    [SerializeField]LayerMask playerLayer;
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
        {  Debug.Log("ShopUp");
            //Open ShopList  
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (playerLayer.ContainCheck(other.gameObject.layer))
        {
            Debug.Log("ShopClose");
            //Close ShopList
        }
    }


    public void ShopListPopUp()
    {

    }
}
