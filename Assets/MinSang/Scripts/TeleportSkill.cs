using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TeleportSkill : Skill, IPunObservable
{
    public float teleportDistance = 10f;
    public GameObject teleportEffectPrefab; // 텔레포트 이펙트 프리팹
    public float effectDuration = 1.0f;
    public LayerMask GroundLayer; // 지형 레이어를 설정하여 맵 밖으로 나가는 것을 방지
    PhotonView pv;
    CharacterController characterController;
    public override void SkillOn()
    {
        Debug.Log(SkillName + "SkillOn");
    }

    public override void SkillOff()
    {
        Debug.Log(SkillName + "SkillOff");
    }

    void Start()
    {
        pv = GetComponent<PhotonView>();
        characterController = GetComponent<CharacterController>();
        if (pv == null)
        {
            Debug.Log("pv를 찾을 수 없습니다.");
        }
        else if (pv.IsMine)
        {
            pv.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        if (pv.IsMine && Input.GetKeyDown(KeyCode.T))
        {
            Teleport();
        }
    }

    void Teleport()
    {
        Vector3 targetPosition = transform.position + transform.forward * teleportDistance;

        if (IsValidTeleportPosition(targetPosition))
        {
            pv.RPC("RPC_Teleport", RpcTarget.All, targetPosition);
            transform.position = targetPosition;
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
        Debug.Log("RPC_Teleport called with position: " + targetPosition);
        // 시작 이펙트 생성
        if (teleportEffectPrefab != null)
        {
            GameObject startEffect = Instantiate(teleportEffectPrefab, transform.position, Quaternion.identity);
            Destroy(startEffect, effectDuration);
        }

        // Character Controller를 사용하여 위치 이동
        if (characterController != null)
        {
            characterController.enabled = false; // 일시적으로 Character Controller 비활성화
            transform.position = targetPosition;
            characterController.enabled = true; // 다시 활성화
        }
        else
        {
            transform.position = targetPosition;
        }

        Debug.Log("Player position set to: " + transform.position);
        // 새로운 위치에 텔레포트 이펙트 생성
        if (teleportEffectPrefab != null)
        {
            GameObject endEffect = Instantiate(teleportEffectPrefab, targetPosition, Quaternion.identity);
            Destroy(endEffect, effectDuration);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 로컬 플레이어의 데이터를 다른 클라이언트에 보냄
            stream.SendNext(transform.position);
        }
        else
        {
            // 다른 클라이언트의 데이터를 수신
            transform.position = (Vector3)stream.ReceiveNext();
        }
    }
}
