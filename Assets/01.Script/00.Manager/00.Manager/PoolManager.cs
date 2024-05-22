using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    private Dictionary<int, ObjectPool> poolDic = new Dictionary<int, ObjectPool>();

    [SerializeField] PooledObject bloodEffect;
    [SerializeField] PooledObject BulletMark;
    [SerializeField] PooledObject BulletSpark; // 벽에 부딪히면 생길 스파크 
    [SerializeField] PooledObject Bullet;  // 실제 날아갈 총알. 

    [SerializeField] private int poolSize = 20;
    [SerializeField] private int poolCapacity = 20;


    private void Start()
    {
        string bloodPath = "BloodEffect";
        bloodEffect = Manager.Resource.basicLoad<PooledObject>(bloodPath);
        if (bloodEffect != null)
        {
            Manager.Pool.CreatePool(bloodEffect, poolSize, poolCapacity);
        }

        string markPath = "BulletMark"; //위랑 똑같은 작업 실시. 
        BulletMark = Manager.Resource.basicLoad<PooledObject>(markPath);
        if (BulletMark != null)
        {
            Manager.Pool.CreatePool(BulletMark, poolSize, poolCapacity);
        }

        string sparkPath = "BulletSpark";
        BulletSpark = Manager.Resource.basicLoad<PooledObject>(sparkPath);
        if (BulletSpark != null)
        {
            Manager.Pool.CreatePool(BulletSpark, poolSize, poolCapacity);
        }

        string bulletPath = "Bullet";
        Bullet = Manager.Resource.basicLoad<PooledObject>(bulletPath);
        if (Bullet != null)
        {
            Manager.Pool.CreatePool(Bullet, poolSize, poolCapacity);
        }

    }


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


    // 아래는 원본 함수들 

    public void CreatePool(PooledObject prefab, int size, int capacity)
    {
        int prefabKey = prefab.GetInstanceID();
        if (!poolDic.ContainsKey(prefabKey)) // 이미 있으면 안함
        {
            GameObject gameObject = new GameObject();
            gameObject.name = $"Pool_{prefab.name}";

            ObjectPool objectPool = gameObject.AddComponent<ObjectPool>();
            objectPool.CreatePool(prefab, size, capacity);

            poolDic.Add(prefabKey, objectPool);
        }
    }

    public void DestroyPool(PooledObject prefab)
    {
        ObjectPool objectPool = poolDic[prefab.GetInstanceID()];
        Destroy(objectPool.gameObject);

        poolDic.Remove(prefab.GetInstanceID());
    }

    public void ClearPool()
    {
        foreach (ObjectPool objectPool in poolDic.Values)
        {
            Destroy(objectPool.gameObject);
        }

        poolDic.Clear();
    }

    public PooledObject GetPool(PooledObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null)
            return null;
        int id = prefab.GetInstanceID();
        if (poolDic.ContainsKey(id))
            return poolDic[id].GetPool(position, rotation);
        return null;
    }
    public PooledObject GetPool(int prefabId , Vector3 position, Quaternion rotation)
    {
        if (poolDic.ContainsKey(prefabId))
            return poolDic[prefabId].GetPool(position, rotation);
        else
            return null;
    }

}
