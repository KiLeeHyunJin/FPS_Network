using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public class Gun : IKWeapon, IPunObservable
{
    // 모든 종류의 총들이 공통적으로 갖고 있는 속성들 


    public enum GunType 
    {
        SHOTGUN,AR,SNIPER, BASICPISTOL, PURCHASEPISTOL,END
    }

    [Tooltip("ui에 표현될 총의 스프라이트")]
    public Sprite gunSprite;


    [Tooltip("총의 ID 넘버")]
    public int gunID;

    [Tooltip("총의 종류 -> 권총, AR, 저격총")]
    public GunType gunType;
    [Header("총의 기본 스펙")]
    [Tooltip("총의 이름")]
    public string gunName; 
    [Tooltip("총의 사정거리")]
    public float range;
    [Tooltip("총의 정확도")]
    public float accuracy; //총의 정확도. 총의 종류마다 정확도가 다름. 
    [Tooltip("총의 연사속도")]
    public float fireRate; //연사속도 --> 총의 종류마다 연사 속도가 다름. 
    [Tooltip("총의 재장전 속도")]
    public float reloadTime; //재장전 속도 
    [Tooltip("총의 트레일 렌더러 ")] //실제 있어야 하나? 
    public TrailRenderer trailEffect;
    [Tooltip("총의 데미지")]
    public int damage; //총의 공격력

    public int otherBullet;

    [Header("총의 총알 갯수 관련")]

    [Tooltip("탄창에 남아있는 총알의 개수")]
    public int currentBulletCount; //현재 탄창에 남아있는 총알의 개수 
    [Tooltip("총알 최대 소유 개수")] //필요함. 
    public int maxBulletCount; // 총알 최대 소유 갯수 


    [Header("반동 세기")]
    public float retroActionForce; //반동의 세기 --> 연사로 하면 반동이 조금 더 강해짐.
    [Tooltip("정조준 시의 반동 세기 ex)스나이퍼")]
    public float retroActionFineSightForce; //정조준시의 반동 세기 (이 게임에서의 정조준이 존재하는지? )

    public Vector3 fineSightOriginPos; //정조준시 총이 향할 위치 -->정조준할 때 총이 변하는 위치.

    [Header("기타 이펙트 및 애니메이션")]
    public Animator anim; //총의 애니메이션을 재생할 애니메이터
    public ParticleSystem muzzleFlash; //화염구 이펙트 재생 담당 -->파티클 시스템 
    public AudioClip fire_Sound; //총 발사 소리 오디오클립 
    public AudioClip reload_Sound; //총 발사 소리 오디오클립 
    public AudioClip dryFire_Sound; //총 발사 소리 오디오클립 


    protected override void Awake()
    {
        base.Awake();
    }
    public void SetData(int current, int other)
    {
        currentBulletCount = current;
        otherBullet = other;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(currentBulletCount);
            stream.SendNext(otherBullet);
        }
        else
        {
            currentBulletCount = (int)stream.ReceiveNext();
            otherBullet = (int)stream.ReceiveNext();
        }
    }
}
