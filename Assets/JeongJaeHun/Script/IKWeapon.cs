using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKWeapon : PooledObject
{
    // 모든 무기가 상속을 해줘야하는 IkWeapon (역운동학을 위해서 )
    // 플레이어가 맞춰줘야 하니까. 각 무기들에 달아줘야함. 
    // leftGrip  // RightGrip 에 각각 자기 부품 위치 넣어주기.

    // 칼 과 수류탄이라면 Right만 넣어주기? 
    [field: SerializeField] public Transform WeaponPos { get; private set; }
    [field : SerializeField] public Transform leftGrip { get; private set; }
    [field : SerializeField] public Transform RightGrip { get; private set; }
    [field: SerializeField] public Transform ZoomPos { get; private set; }
    [field: SerializeField] public AnimationController.AnimatorWeapon weaponType { get; private set; }

    [SerializeField]
    public Quaternion OriginRot { get; protected set; }
    [SerializeField]
    public Vector3 OriginPos { get; protected set; }


    protected virtual void Awake()
    {
        OriginPos = transform.localPosition; 
        OriginRot = transform.localRotation;
    }

}
