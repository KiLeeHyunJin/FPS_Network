using UnityEngine;
using Photon.Pun;

public class MineSkill : MonoBehaviourPun
{
    public GameObject minePrefab;
    public Transform mineSpawnPoint;
    public int maxMines = 3;
    private int currentMines;

    void Start()
    {
        currentMines = maxMines;
    }

    void Update()
    {
        // 지뢰 설치
        if (Input.GetKeyDown(KeyCode.G) && currentMines > 0)
        {
            PlaceMine();
        }
    }

    void PlaceMine()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject mine = PhotonNetwork.Instantiate(minePrefab.name, mineSpawnPoint.position, mineSpawnPoint.rotation, 0);
            currentMines--;
        }
    }

    // 지뢰 충전 메서드
    public void RechargeMines()
    {
        currentMines = maxMines;
    }
}