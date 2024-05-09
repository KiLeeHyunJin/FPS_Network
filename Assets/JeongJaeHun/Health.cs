using System;
using Unity.Mathematics;
using UnityEngine;

public class Health : MonoBehaviour, IInteractable
{
    // 힐 팩 --> 바닥에 있을 때는 interaction으로 구현하고
    // 힐 스킬 관련은 그냥 public 함수로 구현하자. 
    private float hp;
    private float healPack; //힐팩으로 치유되는 힐량. 
    private float maxHp;

    private void Start()
    {
        healPack = 50; //한 50정도 치유됨. 
        maxHp = 100;
    }

    public float Hp  //일단 플레이어의 hp만 구현하자. 
    {
        get => hp = Mathf.Clamp(hp,0,maxHp); //일단 최소체력 0 최대체력 100으로 
    }


    public void Interaction()
    {
        hp += healPack;  // 힐팩의 회복량 만큼 힐을 시전함. 
    }

    public void SkillHeal(float healing) //매개변수에 힐량 받아서 hp 증가 시키기. 
    {
        hp += healing;
    }
}
