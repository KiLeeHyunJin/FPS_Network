using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    public enum ResourceType
    {
        Audio, Weapon, Input, Data,
    }

    private Dictionary<ResourceType, Dictionary<string, Object>> resourceDic = 
        new Dictionary<ResourceType, Dictionary<string, Object>>();

    public void Clear() => resourceDic.Clear();

    public T Load<T>( ResourceType type, string path ) where T : Object
    {
        string key = $"{path}_{typeof(T)}";

        if(resourceDic.TryGetValue(type, out Dictionary<string,Object> dic))
        {
            if(dic.TryGetValue(path, out Object obj))
            {
                return obj as T;
            }
        }
        if(dic == null)
        {
            resourceDic.Add(type, new Dictionary<string,Object>());
        }
        T resource = Resources.Load<T>(path);
        resourceDic[type].Add(key, resource);
        return resource;
    }
}
