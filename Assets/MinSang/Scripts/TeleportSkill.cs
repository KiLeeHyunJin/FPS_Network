using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TeleportSkill : MonoBehaviourPun
{
    public float teleportDistance = 10f;
    public GameObject teleportEffectPrefab; // 텔레포트 이펙트 프리팹
    public float effectDuration = 1.0f;
    public LayerMask groundLayer; // 지형 레이어를 설정하여 맵 밖으로 나가는 것을 방지

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

        // 레이캐스트를 사용하여 목표 위치가 맵 안에 있는지 확인
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
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // 레이캐스트가 지형에 닿았는지 확인
        if (Physics.Raycast(ray, out hit, teleportDistance, groundLayer))
        {
            // 목표 위치가 레이캐스트가 닿은 지점 근처인지 확인
            return Vector3.Distance(hit.point, targetPosition) < 1.0f;
        }

        return false;
    }

    [PunRPC]
    void RPC_Teleport(Vector3 targetPosition)
    {
        GameObject startEffect = Instantiate(teleportEffectPrefab, transform.position, Quaternion.identity);
        Destroy(startEffect, effectDuration);

        // 플레이어 위치를 타겟 위치로 이동
        transform.position = targetPosition;

        // 새로운 위치에 텔레포트 이펙트 생성
        GameObject endEffect = Instantiate(teleportEffectPrefab, targetPosition, Quaternion.identity);
        Destroy(endEffect, effectDuration);
    }
}