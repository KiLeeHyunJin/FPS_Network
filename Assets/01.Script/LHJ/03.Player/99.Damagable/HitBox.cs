using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour, IDamagable, IDamagableNaming
{
    Controller controller;
    public void SetOwner(Controller _controller, bool mine)
    {
        controller = _controller;
        Collider collider = GetComponent<Collider>();
        this.gameObject.layer = LayerMask.NameToLayer("HitBox");
        if (collider != null && collider.isTrigger == false)
            collider.isTrigger = true;
    }
    public void TakeDamage(int _damage)
        => controller.Damage(_damage);

    public void TakeDamage(int _damage, int _actorNumber)
    {
        
        controller.Damage(_damage, _actorNumber);      
    }

    

}
