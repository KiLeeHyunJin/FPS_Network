using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolContainer : MonoBehaviour
{
    [SerializeField] PooledObject bloodEffect;
    [SerializeField] PooledObject BulletMark;
    [SerializeField] PooledObject BulletSpark; // 벽에 부딪히면 생길 스파크 

    private void Awake()
    {
        string bloodPath = "BloodEffect";
        bloodEffect = Manager.Resource.basicLoad<PooledObject>(bloodPath);
        if (bloodEffect != null)
        {
            Manager.Pool.CreatePool(bloodEffect, 10, 10);
        }

        string markPath = "BulletMark"; //위랑 똑같은 작업 실시. 


        string sparkPath = "BulletSpark";




    }

    public void GetBloodEffect(Vector3 pos, Quaternion quaternion)
    {
        // 생성 위치와 회전은 실제 부르는 곳에서 매개변수로 넣어주기. 
        // 위치는-> 피격 위치 회전값은 -> 노멀벡터로 총알 맞는 방향 ( + normal 값) 
        Manager.Pool.GetPool(bloodEffect, pos, quaternion);
    }

    public void GetbulletMarks(Vector3 pos, Quaternion quaternion)
    {

    }

    public void GetBulletSpark(Vector3 pos, Quaternion quaternion)
    {

    }





}
