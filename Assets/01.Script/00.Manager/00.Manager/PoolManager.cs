using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    private Dictionary<int, ObjectPool> poolDic = new Dictionary<int, ObjectPool>();
    private Dictionary<int, ObjectPool> itemDic = new Dictionary<int, ObjectPool>();

    [SerializeField] PooledObject bloodEffect;
    [SerializeField] PooledObject BulletMark;
    [SerializeField] PooledObject BulletSpark; // 벽에 부딪히면 생길 스파크 
    [SerializeField] PooledObject Bullet;  // 실제 날아갈 총알. 
    [SerializeField] public IKWeapon AR;  // 실제 날아갈 총알. 
    [SerializeField] private int poolSize = 20;
    [SerializeField] private int poolCapacity = 20;


   /* private void Start()
    {
        string bloodPath = "BloodEffect";
        bloodEffect = Manager.Resource.basicLoad<PooledObject>(bloodPath);
        if (bloodEffect != null)
        {
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

    }*/



   /* public void GetBloodEffect(Vector3 pos, Quaternion quaternion)
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

    public PooledObject GetBullet(Vector3 pos, Quaternion quaternion) //총구에서 Fire 시 나갈 총알. 
    {
          return Manager.Pool.GetPool(Bullet, pos, quaternion);
    }
*/

    // 아래는 원본 함수들 

    public void CreateBasicPool(PooledObject prefab, int size, int capacity)
    {
        CreateObj(prefab, prefab.GetInstanceID(), size, capacity, poolDic);
    }


    void CreateObj(PooledObject prefab, int key, int size, int capacity, Dictionary<int, ObjectPool> dic)
    {
        if (!dic.ContainsKey(key)) // 이미 있으면 안함
        {
            GameObject gameObject = new GameObject();
            gameObject.name = $"Pool_{prefab.name}";

            ObjectPool objectPool = gameObject.AddComponent<ObjectPool>();
            objectPool.CreatePool(prefab, size, capacity);

            dic.Add(key, objectPool);
        }
    }





    public void DestroyBasicPool(PooledObject prefab)
    {
        DestroyObj(prefab.GetInstanceID(), poolDic);
    }
    public void DestroyItem(IKWeapon prefab)
    {
        DestroyObj(prefab.InstanceId, itemDic);
    }
    void DestroyObj(int key, Dictionary<int, ObjectPool> dic)
    {
        if(dic.ContainsKey(key))
        {
            ObjectPool objectPool = poolDic[key];
            Destroy(objectPool.gameObject);
            poolDic.Remove(key);
        }
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
        return GetObj(id, position, rotation, poolDic);
    }


    public PooledObject GetPool(int prefabId , Vector3 position, Quaternion rotation)
    {
        return GetObj( prefabId,  position,  rotation, poolDic);
    }

    public PooledObject GetItem(int prefabId, Vector3 position, Quaternion rotation)
    {
        return GetObj(prefabId, position, rotation, itemDic);
    }

    PooledObject GetObj(int prefabId, Vector3 position, Quaternion rotation, Dictionary<int, ObjectPool> dic)
    {
        if (dic.ContainsKey(prefabId))
            return dic[prefabId].GetPool(position, rotation);
        else
            return null;
    }
}
