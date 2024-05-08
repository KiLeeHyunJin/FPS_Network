using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerPanel : MonoBehaviour
{
    [SerializeField] ServerSettings photonServer;
    [SerializeField] string[] serverId;

    private void Awake()
    {
        
    }


    private void Start()
    {
        photonServer.AppSettings.Protocol = ExitGames.Client.Photon.ConnectionProtocol.Udp;
        photonServer.AppSettings.AppIdRealtime = "79144f98-5bec-4f07-a940-a33afe6a79e8";
        photonServer.AppSettings.FixedRegion = "kr";
    }
}
