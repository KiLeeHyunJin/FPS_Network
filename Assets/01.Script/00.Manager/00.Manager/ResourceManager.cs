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

    
    //정재훈 : 추가 버전 (기본 리소스 매니저 버전 )
    private Dictionary<string,Object> basicResources=new Dictionary<string,Object>();

    public T basicLoad<T>(string path) where T: Object
    {
        return LoadTree<T>(path, basicResources);
    }


    private Dictionary<string, Object> ItemResources = new Dictionary<string, Object>();
    public T LoadItem<T>(string path) where T : Object
    {
        return LoadTree<T>(path, ItemResources);
    }

    public T LoadTree<T>(string path, Dictionary<string, Object> resouceTree ) where T : Object
    {
        string key = $"{path}_{typeof(T)}";

        if (resouceTree.TryGetValue(key, out Object obj))
        {
            return obj as T;
        }
        else
        {
            T resource = Resources.Load<T>(path);
            resouceTree.Add(key, resource);
            return resource;
        }
    }
}
