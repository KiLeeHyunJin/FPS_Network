using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GunItem : EquipItem
{
    [field: SerializeField] 
    public Define.Gun gunType 
    { get; protected set; }
    public Define.Bullet bulletType
    { get; protected set; }
    protected int currentBulletCount;
    protected int maxBulletCount;
    protected int damage;
    protected float speed;
    [SerializeField] Transform muzzle;
    [SerializeField] BulletItem bulletPrefab;
    public override void GetItem()
    {
        base.GetItem();
    }
    public override void Used(int count)
    {
        if (currentBulletCount <= 0)
            return;
        currentBulletCount--;
        Fire();
    }
    public override void Dequip()
    {

    }
    protected void Fire()
    {
        BulletItem bullet = (BulletItem)Manager.Pool.GetPool(bulletPrefab, muzzle.position, muzzle.rotation);
    }
    protected abstract void SetData();

    public void SetMaxBulletCount(int count)
        => maxBulletCount = count;
    public int GetEmptyBulletCount()
        => maxBulletCount - currentBulletCount;
    public int AddBulletCount(int count)
    {
        int other = 0;
        currentBulletCount += count;
        if (currentBulletCount > maxBulletCount)
        {
            other = currentBulletCount - maxBulletCount;
            currentBulletCount = maxBulletCount;
        }
        return other;
    }
} 
