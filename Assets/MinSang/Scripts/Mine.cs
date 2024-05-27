using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Photon.Pun;

public class Mine : MonoBehaviourPun, IPunObservable
{
    [SerializeField]
    private float explosionRadius = 0.5f; // 폭발 범위
    [SerializeField]
    private float explosionForce = 10.0f;
    [SerializeField]
    private float damage = 50f;
    [SerializeField]
    private GameObject explosionEffectPrefab; // 폭발 이펙트 프리팹

    private bool hasExploded = false;
    private SphereCollider detectionCollider;

    private void Start()
    {
        detectionCollider = gameObject.AddComponent<SphereCollider>();
        detectionCollider.isTrigger = true;
        detectionCollider.radius = explosionRadius;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasExploded && other.CompareTag("Player"))
        {
            Debug.Log("지뢰 폭발");
            hasExploded = true;
            Explode();
        }
    }

    private void Explode()
    {
        photonView.RPC("RPC_Explode", RpcTarget.All);
    }

    [PunRPC]
    public void RPC_Explode()
    {
        // 폭발 이펙트 생성
        if (explosionEffectPrefab != null)
        {
            GameObject explosionEffect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

            ParticleSystem particleSystem = GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Play();
            }

            Destroy(explosionEffect, 2.0f); // 이펙트 객체 2초 후 제거
        }

        // 폭발 처리
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            if (hit.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

            if (hit.TryGetComponent<IDamagable>(out IDamagable damagable))
            {
                damagable.TakeDamage((int)damage);
            }
        }

        // 지뢰 객체 제거
        PhotonNetwork.Destroy(gameObject);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(hasExploded);
        }
        else
        {
            hasExploded = (bool)stream.ReceiveNext();
        }
    }

    // 사용자 정의를 위한 속성들
    public float ExplosionRadius { get => explosionRadius; set => explosionRadius = value; }
    public float ExplosionForce { get => explosionForce; set => explosionForce = value; }
    public float Damage { get => damage; set => damage = value; }
}
