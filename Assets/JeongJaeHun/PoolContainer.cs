using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolContainer : MonoBehaviour
{
    [SerializeField]PooledObject bloodEffect;

    private void Awake()
    {
        string path = "BloodEffect";
        bloodEffect = Manager.Resource.basicLoad<PooledObject>(path);
        if(bloodEffect != null )
        {
            Manager.Pool.CreatePool(bloodEffect, 10 , 10);
        }
    }

    public void GetBloodEffect(Vector3 pos,Quaternion quaternion)
    {
        // 생성 위치와 회전은 실제 부르는 곳에서 매개변수로 넣어주기. 
        // 위치는-> 피격 위치 회전값은 -> 노멀벡터로 총알 맞는 방향 ( + normal 값) 
        Manager.Pool.GetPool(bloodEffect, pos, quaternion); 
    }




}
