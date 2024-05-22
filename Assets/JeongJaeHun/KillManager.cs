using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.Events;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class KillManager : MonoBehaviourPunCallbacks,IPunObservable
{
    // 정재훈 : 여기서 죽인 사람, 죽는 사람 출력 및 kill Count 등 저장해놓자. 


   public static UnityEvent<Player,Player> OnKilled
        =new UnityEvent<Player, Player> (); //죽인 사람 , 죽은 사람 

    private void Awake()
    {
        
    }



    public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashtable changedProps)
    {
        
    }

    [PunRPC]
    public string KillMessage(string message)
    {
        return message;
    }

    


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}
