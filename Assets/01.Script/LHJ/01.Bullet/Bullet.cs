using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : PooledObject
{
    (int damage, int id , float moveSpeed) data;
    int teamCode;
    Vector3 beforePos;
    Vector3 direction;
    RaycastHit hitInfo;

    public void SetData(float _moveSpeed, int _damage, int _shooterId, int _shooterTeam, float _time)
    {
        data = (_damage, _shooterId, _moveSpeed);
        //teamCode = _shooterTeam;
        beforePos = transform.position;
        direction = transform.forward;

        HitCheck(_time);
    }
    private void Update()
    {
        HitCheck(Time.deltaTime);
    }
    void Collision()
    {
        if (hitInfo.collider != null)
        {
            if (hitInfo.collider.TryGetComponent<Controller>(out Controller player))
            {
                if(teamCode != player.TeamCode)
                {
                    player.Damage(data.damage);
                }
            }
            else
            {

            }
            Release();
        }
    }
    void HitCheck(float delayTime)
    {
        float moveDistance = (delayTime * data.moveSpeed);
        bool state = Physics.Raycast(beforePos, direction, out hitInfo, moveDistance);
        transform.position += moveDistance * transform.forward;
        beforePos = transform.position;
        if(state)
            Collision();
    }

}
