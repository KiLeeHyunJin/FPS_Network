using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

//[RequireComponent(typeof(Rigidbody))]
public class Bullet : PooledObject
{
    // 총알의 주인이 누구 인지 알아야한다. 
    public int actorNumber; 


    (int damage, float moveSpeed) data;
    int teamCode;
    Vector3 beforePos;
    Vector3 direction;
    RaycastHit hitInfo;

    AudioSource audioSource;

    

    private void OnEnable()
    {
        
        audioSource = GetComponent<AudioSource>(); 
        //Rigidbody rigidbody=GetComponent<Rigidbody>();

        //rigidbody.AddForce(transform.forward*moveForce);
    }

    public void SetData(float _moveSpeed, int _damage, int _shooterTeam, float _time)
    {
        data = (_damage, _moveSpeed);
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
            if (hitInfo.collider.TryGetComponent<IDamagable>(out IDamagable damagable))
            {
                //if(teamCode != player.TeamCode)
                {
                    // bullet 자신의 actornumber이용.

                    damagable.TakeDamage(data.damage,actorNumber);
                    Vector3 pos = hitInfo.point;
                    Quaternion rot = Quaternion.LookRotation(hitInfo.normal);
                    //사람에 맞으면 피 법선벡터로 생성 
                    //Manager.Pool.GetBloodEffect(pos, rot);
                    Release();

                }
            }
            else // 벽 이나 땅에 부딪힌 경우 mark + spark 프리팹을 불러와줘야함. 
            {
                Vector3 pos = hitInfo.point;
                Quaternion rot = Quaternion.LookRotation(hitInfo.normal);
                //Manager.Pool.GetbulletMarks(pos, rot);
                //Manager.Pool.GetBulletSpark(pos, rot);
                Release();

            }
        }
    }
    void HitCheck(float delayTime)
    {
        float moveDistance = (delayTime * data.moveSpeed);
        bool state = Physics.Raycast(beforePos, direction, out hitInfo, moveDistance);
        transform.position += moveDistance * transform.forward;
        beforePos = transform.position;
        if (state)
            Collision();
    }




}
