using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManagerStarter : MonoBehaviourPunCallbacks
{
   
    [SerializeField] Transform redLocation;
    [SerializeField] Transform blueLocation;
    [SerializeField] int readyPlayer;


    void Start()
    {
        if (PhotonNetwork.InRoom == false)
            return;

        AllPlayerReadyCheck();
       
    }

    void AllPlayerReadyCheck()
    {
        
        PhotonNetwork.LocalPlayer.SetProperty(DefinePropertyKey.LOADCOMPLETE, true);
        StartCoroutine(WaitForPlayers());
        
    }
    IEnumerator WaitForPlayers()
    {
        while (!AllReady(DefinePropertyKey.LOADCOMPLETE))
        {
            Debug.Log($"Ready on {readyPlayer}, listLength is {PhotonNetwork.PlayerList.Length}");
            yield return null;
        }
        Debug.Log($"Ready on {readyPlayer}, listLength is {PhotonNetwork.PlayerList.Length}");
        Debug.Log("AllPlayerReady");
        Manager.Scene.StartFadeIn();
        Manager.Game.StartGame(blueLocation, redLocation);

    }

    public bool AllReady(string key)
    {
        for(int i = 0; i< PhotonNetwork.PlayerList.Length; i++)
            if (PhotonNetwork.PlayerList[i].CustomProperties[key] == null)
                return false;
        return true;
        
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {

    }
}
