using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealPack : MonoBehaviourPun,IInteractable
{
    [SerializeField] int heal = 50;
    

    public void Interaction(GameObject player)
    {
        Controller controller= player.GetComponent<Controller>();
        
        //Controller 컴포넌트가 있다면
        if(controller != null && photonView.IsMine)
        {
            controller.AddHp(heal);
        }

        PhotonNetwork.Destroy(gameObject);
    }

  


}
