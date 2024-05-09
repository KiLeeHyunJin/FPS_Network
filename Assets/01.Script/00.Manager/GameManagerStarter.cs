using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerStarter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.InRoom == false)
            return;
        Manager.Game.Spawn(null);
        Manager.Game.StartGame();
    }
}
