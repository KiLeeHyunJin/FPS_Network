using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontSensor : MonoBehaviour
{
    [SerializeField] List<Collider> colliders;
    Coroutine co;
    public Collider FrontObj 
    { 
        get 
        {
            for (int i = colliders.Count - 1; i > -1; i--)
            {
                if (colliders[i] != null)
                    return colliders[i];
            }
            return null;
        } 
    }

    private void Awake()
    {
        colliders = new List<Collider>(5);
    }

    IEnumerator NullCheckRoutine()
    {
        while(true)
        {
            for (int i = 0; i < colliders.Count; i++)
            {
                if(colliders[i].gameObject.activeSelf == false)
                    colliders.RemoveAt(i);
            }
            if (colliders.Count == 0)
                yield break;
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        colliders.Add(other);
        co ??= StartCoroutine(NullCheckRoutine());
    }

    private void OnTriggerExit(Collider other)
    {
        colliders.Remove(other);
    }


}
