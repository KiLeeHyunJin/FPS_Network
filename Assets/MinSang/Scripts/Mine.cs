using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using static Define;

public class Mine : MonoBehaviour
{
    [SerializeField]
    private float explosionRadius = 5.0f;
    [SerializeField]
    private float explosionForce = 10.0f;
    [SerializeField]
    private float damage = 50.0f;

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
            hasExploded = true;
            Explode();
        }
    }

    private void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

            IDamagable damagable = hit.GetComponent<IDamagable>();
            if (damagable != null)
            {
                damagable.TakeDamage((int)damage);
            }
        }

        Destroy(gameObject);
    }

    // 사용자 정의를 위한 속성들
    public float ExplosionRadius { get => explosionRadius; set => explosionRadius = value; }
    public float ExplosionForce { get => explosionForce; set => explosionForce = value; }
    public float Damage { get => damage; set => damage = value; }
}
