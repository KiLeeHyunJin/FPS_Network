using UnityEngine;
using Photon.Pun;


public class MineSkill : MonoBehaviourPun
{
    [SerializeField]
    private GameObject minePrefab;
    [SerializeField]
    private float cooldownTime = 5.0f;
    [SerializeField]
    private float spawnDistance = 2.0f; // 플레이어 앞의 지뢰 생성 거리

    private float nextMineTime = 3f;

    public void Activate()
    {
        if (Time.time > nextMineTime)
        {
            nextMineTime = Time.time + cooldownTime;

            // 플레이어 앞의 위치 계산
            Vector3 spawnPosition = transform.position + transform.forward * spawnDistance;
            Instantiate(minePrefab, spawnPosition, Quaternion.identity);
        }
    }

    void Update()
    {
        if (minePrefab == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            Activate();
        }
    }

    // 사용자 정의를 위한 속성들
    public GameObject MinePrefab { get => minePrefab; set => minePrefab = value; }
    public float CooldownTime { get => cooldownTime; set => cooldownTime = value; }
    public float SpawnDistance { get => spawnDistance; set => spawnDistance = value; }
}