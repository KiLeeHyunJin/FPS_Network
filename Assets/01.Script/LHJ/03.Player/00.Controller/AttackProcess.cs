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


    public Controller Attack()
    {
        if (sleepTime != 0)
            sleepTime = 0;

        if (aim == null)
            return null;

        //Recoil();

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hitInfo, Mathf.Infinity))
        {
            if (hitInfo.collider != null)
            {
                Vector3 direction = (hitInfo.transform.position - aim.position).normalized;

                if (Physics.Raycast(aim.position, direction, out hitInfo, Mathf.Infinity))
                    return hitInfo.collider.GetComponent<Controller>();
            }
        }
        return null;
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
