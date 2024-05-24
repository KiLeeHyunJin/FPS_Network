using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PoolContainer : MonoBehaviourPun //어차피 같은 이펙트 쓰니까 그냥 싱글톤 상속해서 만들자 (부모에 photonPun 달려있음) 
{
    [SerializeField] PooledObject bloodEffect;
    [SerializeField] PooledObject BulletMark;
    [SerializeField] PooledObject BulletSpark; // 벽에 부딪히면 생길 스파크 
    [SerializeField] PooledObject Bullet;  // 실제 날아갈 총알. 

    [SerializeField]private int poolSize = 20;
    [SerializeField]private int poolCapacity = 20;


    private void Awake()
    {
        Debug.Log("시작");
        string bloodPath = "BloodEffect";
        bloodEffect = Manager.Resource.basicLoad<PooledObject>(bloodPath);
        if (bloodEffect != null)
        {
            Debug.Log("생성");
            Manager.Pool.CreateBasicPool(bloodEffect, poolSize, poolCapacity);
        }

        string markPath = "BulletMark"; //위랑 똑같은 작업 실시. 
        BulletMark = Manager.Resource.basicLoad<PooledObject>(markPath);
        if (BulletMark != null)
        {
            Manager.Pool.CreateBasicPool(BulletMark, poolSize, poolCapacity);
        }

        string sparkPath = "BulletSpark";
        BulletSpark = Manager.Resource.basicLoad<PooledObject>(sparkPath);
        if (BulletSpark != null)
        {
            Manager.Pool.CreateBasicPool(BulletSpark, poolSize, poolCapacity);
        }

        string bulletPath = "Bullet";
        Bullet = Manager.Resource.basicLoad<PooledObject>(bulletPath);
        if (Bullet != null)
        {
            Manager.Pool.CreateBasicPool(Bullet, poolSize, poolCapacity);
        }

    }

    // 만약 참조해서 함수로 가져와야 할 일이 있으면 실행. 
    public void GetBloodEffect(Vector3 pos, Quaternion quaternion)
    {
        // 생성 위치와 회전은 실제 부르는 곳에서 매개변수로 넣어주기. 
        // 위치는-> 피격 위치 회전값은 -> 노멀벡터로 총알 맞는 방향 ( + normal 값) 
        Manager.Pool.GetPool(bloodEffect, pos, quaternion);
    }

    public void GetbulletMarks(Vector3 pos, Quaternion quaternion) //총알 자국 
    {
        Manager.Pool.GetPool(BulletMark, pos, quaternion);
    }

    public void GetBulletSpark(Vector3 pos, Quaternion quaternion) // 총알 스파크 
    {
        Manager.Pool.GetPool(BulletSpark, pos, quaternion);
    }

    public void GetBullet(Vector3 pos, Quaternion quaternion) //총구에서 Fire 시 나갈 총알. 
    {
        Manager.Pool.GetPool(Bullet, pos, quaternion);
    }




}
