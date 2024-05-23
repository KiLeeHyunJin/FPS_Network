using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontSensor : MonoBehaviour
{
    [SerializeField] List<Collider> colliders;
    Coroutine co;
    bool alive;
    readonly string IgnoreLayer = "Ignore";
    readonly string WeaponLayer = "Weapon";
    int ignoreLayer;
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
        ignoreLayer = LayerMask.NameToLayer(IgnoreLayer);
    }
    public void StartInit()
    {
        alive = true;
    }
    public void StopRoutine()
    {
        alive = false;
        if (co != null)
            StopCoroutine(co);
    }

    IEnumerator NullCheckRoutine()
    {
        while(true)
        {
            for (int i = 0; i < colliders.Count; i++)
            {
                if (colliders[i] == null)
                    colliders.RemoveAt(i);
            }
            if (colliders.Count == 0)
                yield break;
            else if(colliders.Count > 1)
            colliders.Sort(Collider);
            yield return new WaitForSeconds(0.5f);
        }
    }

    int Collider(Collider a, Collider b)
    {
        if (Vector3.Distance(a.transform.position, transform.position) < 
            Vector3.Distance(b.transform.position, transform.position))
        {
            return -1;
        }
        return 1;
    }

private void OnTriggerEnter(Collider other)
    {
        if (alive == false)
            return;
        colliders.Add(other);
        co ??= StartCoroutine(NullCheckRoutine());
    }

    private void OnTriggerExit(Collider other)
    {
        if (alive == false)
            return;
        colliders.Remove(other);
    }


}
