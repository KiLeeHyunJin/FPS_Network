using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TeleportSkill : MonoBehaviourPun
{
    public float teleportDistance = 10f;
    public GameObject teleportEffectPrefab; // 텔레포트 이펙트 프리팹
    public float effectDuration = 1.0f;
    public LayerMask GroundLayer; // 지형 레이어를 설정하여 맵 밖으로 나가는 것을 방지

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Teleport();
        }
    }

    void Teleport()
    {
        Vector3 targetPosition = transform.position + transform.forward * teleportDistance;

        if (IsValidTeleportPosition(targetPosition))
        {
            photonView.RPC("RPC_Teleport", RpcTarget.All, targetPosition);
        }
        else
        {
            Debug.Log("Invalid teleport position: Out of bounds.");
        }
    }

    bool IsValidTeleportPosition(Vector3 targetPosition)
    {
        // 목표 위치에서 수직으로 레이캐스트를 쏴서 지형에 닿는지 확인
        Ray ray = new Ray(targetPosition + Vector3.up * 0.5f, Vector3.down);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red);
        // 레이캐스트가 지형에 닿았는지 확인
        if (Physics.Raycast(ray, out hit, 1f, GroundLayer))
        {
            // 목표 위치가 지형 위에 있는지 확인
            return hit.collider != null;
        }

        return false;
    }

    [PunRPC]
    void RPC_Teleport(Vector3 targetPosition)
    {
        // 시작 이펙트 생성
        GameObject startEffect = Instantiate(teleportEffectPrefab, transform.position, Quaternion.identity);
        Destroy(startEffect, effectDuration);

        // 플레이어 위치를 타겟 위치로 이동
        transform.position = targetPosition;

        // 새로운 위치에 텔레포트 이펙트 생성
        GameObject endEffect = Instantiate(teleportEffectPrefab, targetPosition, Quaternion.identity);
        Destroy(endEffect, effectDuration);
    }
}