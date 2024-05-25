using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour, IDamagable, IDamagableNaming
{
    Controller controller;
    [SerializeField] float multiple = 1;
    public int GetActNum { get { return controller.photonView.Controller.ActorNumber; } }
    public void SetOwner(Controller _controller, bool mine)
    {
        controller = _controller;
        Collider collider = GetComponent<Collider>();
        this.gameObject.layer = LayerMask.NameToLayer("HitBox");
        if (collider != null && collider.isTrigger == false)
            collider.isTrigger = true;
    }
    public void TakeDamage(int _damage)
        => controller.Damage((int)(_damage * multiple));

    public void TakeDamage(int _damage, int _actorNumber)
    {
        controller.Damage((int)(_damage * multiple));      
    }

    

}
