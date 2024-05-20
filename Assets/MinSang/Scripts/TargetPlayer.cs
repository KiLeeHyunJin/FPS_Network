using UnityEngine;
using Photon.Pun;

public class TargetPlayer : MonoBehaviourPun, IDamagable
{
    public int hp = 100;

    public void TakeDamage(int damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            // 죽음 처리
            Debug.Log($"플레이어 사망!");
        }
    }
}