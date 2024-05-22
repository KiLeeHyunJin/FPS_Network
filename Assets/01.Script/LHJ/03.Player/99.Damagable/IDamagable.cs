using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable : IDamagableNaming
{
    public void TakeDamage(int _damage);

}
public interface IDamagableNaming 
{
    public void TakeDamage(int _damage, int _actorNumber);

}
