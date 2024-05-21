using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEditor;
using Firebase.Database;

public class Heal : MonoBehaviourPun
{
    Controller Controller;
    DatabaseReference dbReference;

    int maxHealth = 100;
    int currentHealth = 100;

    public void Activate()
    {

    }

    void Start()
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    void Update()
    {
        // 힐 스킬 발동 키 설정
        if (Input.GetKeyDown(KeyCode.H))
        {
            HealSkill(20);
        }
    }

    [PunRPC]
    void HealSkill(int amount)
    {
        if (!photonView.IsMine)
            return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        Debug.Log($"Heal amount: {amount}. Current health: {currentHealth}");

        // 다른 플레이어도 회복하도록 RPC 호출
        photonView.RPC("Heal", RpcTarget.Others, amount);
    }
}