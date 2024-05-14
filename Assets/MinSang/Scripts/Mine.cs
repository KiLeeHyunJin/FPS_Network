using UnityEngine;
using Photon.Pun;

public class Mine : MonoBehaviourPun
{
    public int damageAmount = 50;

    void OnTriggerEnter(Collider other)
    {
        // Photon을 통한 네트워크 동기화
        if (photonView.IsMine)
        {
            PhotonView targetPhotonView = other.GetComponent<PhotonView>();
            if (targetPhotonView != null)
            {
                // 모든 클라이언트에 피해를 전달
                photonView.RPC("ApplyDamage", RpcTarget.All, targetPhotonView.ViewID);
            }

            // 지뢰 파괴
            PhotonNetwork.Destroy(gameObject);
        }
    }

    [PunRPC]
    void ApplyDamage(int targetViewID)
    {
        PhotonView targetView = PhotonView.Find(targetViewID);
        if (targetView != null)
        {
            IDamagable target = targetView.GetComponent<IDamagable>();
            if (target != null)
            {
                target.TakeDamage(damageAmount);
                Debug.Log($"Mine triggered and dealt {damageAmount} damage to {targetView.Owner.NickName}");
            }
        }
    }
}