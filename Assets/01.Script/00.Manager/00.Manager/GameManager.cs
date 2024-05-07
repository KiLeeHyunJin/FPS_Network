using Photon.Pun;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : Singleton<GameManager>
{
    Transform spawner;
    public void Spawn(Transform spawnPos)
    {
        if (spawnPos == null)
            spawnPos = this.transform;
        spawner = spawnPos;
    }

    public void StartGame()
    {
        PhotonNetwork.Instantiate("Player", spawner.transform.position, spawner.rotation, 0);
    }
}
