using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerStarter : MonoBehaviour
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

        Debug.Log("AllPlayerReady");
        Manager.Game.StartGame(blueLocation, redLocation);

    }

    public bool AllReady(string key)
    {
        for(int i = 0; i< PhotonNetwork.PlayerList.Length; i++)
            if (PhotonNetwork.PlayerList[i].CustomProperties[key] == null)
                return false;
        return true;
        
    }
}
