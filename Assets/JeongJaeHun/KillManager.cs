using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.Events;

public class KillManager : MonoBehaviourPunCallbacks
{
   public static UnityEvent<Player,Player> OnKilled
        =new UnityEvent<Player, Player> (); //죽인 사람 , 죽은 사람 

    private void OnEnable()
    {
        // 이벤트 추가 
    }
}
