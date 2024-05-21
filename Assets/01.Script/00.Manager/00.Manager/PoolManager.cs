using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    private Dictionary<int, ObjectPool> poolDic = new Dictionary<int, ObjectPool>();

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
        return poolDic[prefab.GetInstanceID()].GetPool(position, rotation);
    }
}
