using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using static Define;

public class MineSkill : MonoBehaviourPun
{
    [SerializeField]
    private GameObject minePrefab;
    [SerializeField]
    private Transform mineSpawnPoint;
    [SerializeField]
    private float cooldownTime = 5.0f;

    private float nextMineTime = 3f;


    public void Activate()
    {

    }

    void Update()
    {
        if (minePrefab == null || mineSpawnPoint == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.M) && Time.time > nextMineTime)
        {
            nextMineTime = Time.time + cooldownTime;
            Instantiate(minePrefab, mineSpawnPoint.position, mineSpawnPoint.rotation);
        }
    }

    // 사용자 정의를 위한 속성들
    public GameObject MinePrefab { get => minePrefab; set => minePrefab = value; }
    public Transform MineSpawnPoint { get => mineSpawnPoint; set => mineSpawnPoint = value; }
    public float CooldownTime { get => cooldownTime; set => cooldownTime = value; }
}
