using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEditor;
/*
public class Heal : MonoBehaviour
{
    CharacterController characterController;

    int maxHealth = 100;
    int currentHealth = 100;

    void Start()
    {
        // 초기화 코드
    }

    void Update()
    {
        // 캐릭터 이동 관련 코드

        // 힐 스킬 발동 키 설정
        if (Input.GetKeyDown(KeyCode.H))
        {
            Heal(20);
        }
    }

    [PunRPC]
    void Heal(int amount)
    {
        if (!photonView.IsMine)
            return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        Debug.Log($"Heal amount: {amount}. Current health: {currentHealth}");

        // 다른 플레이어도 회복하도록 RPC 호출
        photonView.RPC("Heal", RpcTarget.Others, amount);
    }
} */