using Photon.Pun;
using UnityEngine;

public class Singleton<T> : MonoBehaviourPun where T : MonoBehaviourPun
{
    private static T instance;
    public static T Instance { get { return instance; } }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void CreateInstance()
    {
        T resource = Resources.Load<T>($"Manager/{typeof(T).Name}");
        instance = Instantiate(resource);
    }

    public static void ReleaseInstance()
    {
        if (instance == null)
            return;

        Destroy(instance.gameObject);
        instance = null;
    }
}
