using System;
using System.Collections;
using UnityEngine;

public class AttackProcess
{
    float sleepTime;

    Transform aim;
    Controller owner;
    Coroutine recoilCo;
    public AttackProcess(Controller _owner)
    {
        owner = _owner;
    }
    public void SetAimTransform(Transform _aim)
    => aim = _aim;

    void Recoil()
        => owner.StartCoroutined(RecoilRoutine(), ref recoilCo);


    public Vector3 Attack()
    {
        if (sleepTime != 0)
            sleepTime = 0;

        if (aim == null)
            return Vector3.zero;

        //Recoil();

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hitInfo, Mathf.Infinity))
        {
            if (hitInfo.collider != null)
            {
                return (hitInfo.transform.position - aim.position).normalized;

                //if (Physics.Raycast(aim.position, direction, out hitInfo, Mathf.Infinity))
                //    return hitInfo.collider.GetComponent<Controller>();
            }
        }
        return Vector3.zero;
    }

    public IEnumerator RecoilRoutine()
    {
        while (true)
        {
            sleepTime += Time.deltaTime;
            yield return null;
        }
    }
}
