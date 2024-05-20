using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MineSkill : MonoBehaviourPun, ISkill
{
    [SerializeField]
    private GameObject minePrefab;
    [SerializeField]
    private float cooldownTime;
    Controller controller;
    private bool isCooldown;
    private float nextMineTime = 0.0f;
    public void Activate()
    {

    }

    public void Deactivate()
    {

    }
    private void Update()
    {
        if (minePrefab == null)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.M) && !isCooldown)
        {
            StartCoroutine(PlaceMine());
        }
    }
    // 사용자 정의를 위한 속성들
    public GameObject MinePrefab { get => minePrefab; set => minePrefab = value; }
    public float CooldownTime { get => cooldownTime; set => cooldownTime = value; }

    IEnumerator PlaceMine()
    {
        isCooldown = true;
        Vector3 spawnPosition = transform.position; // 플레이어의 현재 위치
        Quaternion spawnRotation = transform.rotation;
        Instantiate(minePrefab, spawnPosition, spawnRotation);

        yield return new WaitForSeconds(cooldownTime);

        isCooldown = false;
    }
}
