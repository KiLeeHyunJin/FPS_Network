
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UIElements;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : Singleton<GameManager>
{
   [SerializeField]Transform redTeamSpawner;
   [SerializeField]Transform blueTeamSpawner;
    [SerializeField] int spawnRadius = 5;

    const int BLUE = 1;
    const int RED = 2;
    protected override void Awake()
    {base.Awake();
        spawnRadius = 5;
    }
    public void Spawn(Transform blue,Transform red)
    {
        redTeamSpawner = red;
        blueTeamSpawner = blue;
        Vector3 randomDir = Random.onUnitSphere; 
        Vector3 randomPos = randomDir * spawnRadius;

        Vector3 redSpawnPos = red.position + randomPos;
        redSpawnPos.y = 1f;
        Vector3 blueSpawnPos = blue.position + randomPos;
        blueSpawnPos.y = 1f;
        
        redTeamSpawner.position = redSpawnPos;
        blueTeamSpawner.position = blueSpawnPos;
    }


    public void StartGame(Transform b, Transform r)
    {
        Debug.Log("isStart");
        Spawn(b, r);
        if(BLUE ==PhotonNetwork.LocalPlayer.GetPhotonTeam().Code)
        PhotonNetwork.Instantiate("Player", blueTeamSpawner.position, blueTeamSpawner.rotation, 0);
        else
        PhotonNetwork.Instantiate("Player", redTeamSpawner.position, redTeamSpawner.rotation, 0);

    }


}
