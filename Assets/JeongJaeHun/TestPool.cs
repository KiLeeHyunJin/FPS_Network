using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPool : MonoBehaviour
{
    int age;

    private void Start()
    {
        
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            age = Random.Range(1, 100);
            PooledObject bullet=
            Manager.Pool.GetBullet(transform.position, Quaternion.identity);
            bullet.GetComponent<Bullet>().actorNumber = age;
            
            Debug.Log(bullet.GetComponent<Bullet>().actorNumber);
        }
    }



}
