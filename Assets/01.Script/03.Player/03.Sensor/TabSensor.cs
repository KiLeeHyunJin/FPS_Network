using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabSensor : MonoBehaviour
{
    [field: SerializeField]
    public List<Collider> findItem { get; private set; }
    FrontSensor frontSensor;
    SphereCollider sphere;
    Coroutine co;
    int doorLayer;
    private void Awake()
    {
        findItem = new List<Collider>(10);
        sphere = GetComponent<SphereCollider>();
        doorLayer = LayerMask.NameToLayer("Door");
    }
    private void Start()
    {
        frontSensor = GetComponentInChildren<FrontSensor>();
    }
    public void OnTab()
    {
        if (sphere.enabled == false)
            sphere.enabled = true;
    }
    public void OffTab()
    {
        if (sphere.enabled)
            sphere.enabled = false;
        findItem.Clear();
    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            sphere.enabled = !sphere.enabled;
            if (sphere.enabled == false)
                findItem.Clear();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == doorLayer)
            return;
        findItem.Add(other);
        co ??= StartCoroutine(NullCheckRoutine());
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == doorLayer)
            return;
        findItem.Remove(other);
    }
    IEnumerator NullCheckRoutine()
    {
        while (true)
        {
            for (int i = 0; i < findItem.Count; i++)
            {
                if (findItem[i].gameObject.activeSelf == false)
                    findItem.RemoveAt(i);
            }
            if (findItem.Count == 0)
                yield break;
            yield return null;
        }
    }

}
