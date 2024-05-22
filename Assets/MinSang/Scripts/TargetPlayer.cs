using UnityEngine;

public class TargetPlayer : MonoBehaviour, IDamagable
{
    public int hp = 100;

    public void TakeDamage(int damage)
    {
        hp -= damage;
        Debug.Log($"타겟이 {damage} 데미지를 받다. Health is now {hp}.");

        if (hp <= 0)
        {
            // 죽음 처리
            Debug.Log($"플레이어 사망!");
        }
    }

    public void TakeDamage(int _damage, int _actorNumber)
    {
        
    }
}