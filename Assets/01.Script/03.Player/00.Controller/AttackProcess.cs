using System.Collections;
using UnityEngine;

public class AttackProcess
{
    Vector2 kickValue;
    float sleepTime;

    Transform aim;
    public void SetAimTransform(Transform _aim)
    => aim = _aim;

    public Collider Attack()
    {
        if (sleepTime != 0)
            sleepTime = 0;
        if (aim == null)
            return null;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hitInfo, Mathf.Infinity))
        {
            if (hitInfo.collider != null)
            {
                Vector3 direction = (hitInfo.transform.position - aim.position).normalized;

                if (Physics.Raycast(aim.position, direction, out hitInfo, Mathf.Infinity))
                    return hitInfo.collider;
            }
        }
        return null;
    }
    public IEnumerator KickRoutine()
    {
        while (true)
        {
            sleepTime += Time.deltaTime;
            yield return null;
        }
    }
}
