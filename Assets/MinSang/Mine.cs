using UnityEngine;
using Photon.Pun;

public class Mine : MonoBehaviourPun
{
    public float explosionRadius = 5f;
    public float explosionForce = 700f;
    public int damage = 50;
    public GameObject explosionEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && PhotonNetwork.IsMasterClient)
        {
            Explode();
        }
    }

    [PunRPC]
    private void Explode()
    {
        // 폭발 이펙트 생성
        GameObject effect = Instantiate(explosionEffect, transform.position, transform.rotation);
        Destroy(effect, 2f);

        // 주변의 모든 콜라이더에 충격 및 피해 적용
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
            /*
            PlayerHp hp = nearbyObject.GetComponent<PlayerHp>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
            }
            */

            // 물리 효과
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        // 지뢰 파괴
        PhotonNetwork.Destroy(gameObject);
    }

    // 다른 플레이어에게 폭발 신호를 보내는 함수
    public void Detonate()
    {
        photonView.RPC("Explode", RpcTarget.All);
    }
}