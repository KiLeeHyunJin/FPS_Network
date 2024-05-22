using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TeleportSkill : MonoBehaviourPun
{
    public float teleportDistance = 10f;
    public GameObject teleportEffectPrefab; // 텔레포트 이펙트 프리팹
    public float effectDuration = 1.0f;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Teleport();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    void Teleport()
    {
        /*
        Vector3 worldpos = Camera.main.WorldToViewportPoint(this.transform.position);
        if (worldpos.x < 0f) worldpos.x = 0f;
        if (worldpos.y < 0f) worldpos.y = 0f;
        if (worldpos.x > 1f) worldpos.x = 1f;
        if (worldpos.y > 1f) worldpos.y = 1f;
        this.transform.position = Camera.main.ViewportToWorldPoint(worldpos);
        */

        Vector3 targetPosition = transform.position + transform.forward * teleportDistance;
        photonView.RPC("RPC_Teleport", RpcTarget.All, targetPosition);
    }

    [PunRPC]
    void RPC_Teleport(Vector3 targetPosition)
    {
        Instantiate(teleportEffectPrefab, transform.position, Quaternion.identity);
        // Destroy(teleportEffectPrefab,effectDuration);

        transform.position = targetPosition;

        // 새로운 위치에 텔레포트 이펙트 생성
        Instantiate(teleportEffectPrefab, targetPosition, Quaternion.identity);
        // Destroy(teleportEffectPrefab, effectDuration);
    }
}
