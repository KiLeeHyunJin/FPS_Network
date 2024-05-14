using UnityEngine;

public class HandGun : MonoBehaviour
{
    public enum PISTOLGUNType
    {
        BASICPISTOL,PURCHASEPISTOL
    }

    [Tooltip("작은 총의 종류 -->기본 권총, 상점권총")]
    public PISTOLGUNType pistolGunType;
    [Header("권총류의 기본 스펙")]
    [Tooltip("총의 이름")]
    public string gunName;
    [Tooltip("총의 사정거리")]
    public float range;
    [Tooltip("총의 정확도")]
    public float accuracy;
    [Tooltip("총의 재장전 속도")]
    public float reloadTime;
    [Tooltip("총의 트레일 렌더러")]
    public TrailRenderer trailRenderer;
    [Tooltip("총의 데미지")]
    public int damage;

    [Header("총의 총알 갯수 관련 ")]
    [Tooltip("탄창에 남아있는 총알의 개수(현재갯수)")]
    public int currentBulletCount;



}
