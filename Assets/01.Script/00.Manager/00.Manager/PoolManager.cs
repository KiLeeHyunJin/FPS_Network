using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    private Dictionary<int, ObjectPool> poolDic = new Dictionary<int, ObjectPool>();

    public void CreatePool(PooledObject prefab, int size, int capacity)
    {
        GameObject gameObject = new GameObject();
        gameObject.name = $"Pool_{prefab.name}";

        ObjectPool objectPool = gameObject.AddComponent<ObjectPool>();
        objectPool.CreatePool(prefab, size, capacity);

        poolDic.Add(prefab.GetInstanceID(), objectPool);
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
