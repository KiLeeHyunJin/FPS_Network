using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Photon.Pun;

public class Mine : MonoBehaviourPun
{
    [SerializeField]
    private float explosionRadius = 0.5f; // 폭발 범위
    [SerializeField]
    private float explosionForce = 10.0f;
    [SerializeField]
    private float damage = 50f;

    private bool hasExploded = false;
    private SphereCollider detectionCollider;

    void Start()
    {
        detectionCollider = gameObject.AddComponent<SphereCollider>();
        detectionCollider.isTrigger = true;
        detectionCollider.radius = explosionRadius;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!hasExploded && other.CompareTag("Player"))
        {
            Debug.Log("지뢰 폭발");
            hasExploded = true;
            Explode();
            Destroy(gameObject);
        }
    }

    private void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        List<IDamagable> damagables = new List<IDamagable>();

        foreach (Collider hit in colliders)
        {
            if (hit.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

            if (hit.TryGetComponent<IDamagable>(out IDamagable damagable))
            {
                damagables.Add(damagable);
            }
        }

        foreach (var damagable in damagables)
        {
            damagable.TakeDamage((int)damage);
        }

        Destroy(gameObject);
    }

    // 사용자 정의를 위한 속성들
    public float ExplosionRadius { get => explosionRadius; set => explosionRadius = value; }
    public float ExplosionForce { get => explosionForce; set => explosionForce = value; }
    public float Damage { get => damage; set => damage = value; }
}
