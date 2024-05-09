using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour, IDamagable
{
    Controller controller;
    public void SetOwner(Controller _controller)
        => controller = _controller;
    public void TakeDamage(int _damage)
        => controller.Damage(_damage);

    void Start()
    {
        Collider collider = GetComponent<Collider>();
        if (collider != null && collider.isTrigger == false)
            collider.isTrigger = true;
    }

}
