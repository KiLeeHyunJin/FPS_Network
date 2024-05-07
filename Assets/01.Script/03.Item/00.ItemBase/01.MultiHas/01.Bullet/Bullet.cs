using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : PooledObject
{
    (int damage, int id , float moveSpeed) data;
    Coroutine movCo;
    Rigidbody rigid;
    SphereCollider coll;
    private void Awake()
    {
        rigid = gameObject.GetOrAddComponent<Rigidbody>();
        coll = gameObject.GetOrAddComponent<SphereCollider>();
        movCo = null;
        if (coll.isTrigger == false)
            coll.isTrigger = true;
    }

    public void SetData(float _moveSpeed,int _damage, int shooterId)
    {
        data = (_damage, shooterId, _moveSpeed);
        movCo = StartCoroutine(MoveRoutine());
    }
    IEnumerator MoveRoutine()
    {
        while(true)
        {
            rigid.velocity = Vector3.forward * data.moveSpeed;
            yield return new WaitForFixedUpdate();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 6)
            GetComponent<IDamagable>()?.TakeDamage(data.damage);
        StopCoroutine(movCo);
        Release();
    }
}
