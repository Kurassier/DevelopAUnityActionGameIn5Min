using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : CharacterDamage
{
    public override HitResult Hit(Damage damage)
    {
        Debug.Log("Enemy Hit");
        return base.Hit(damage);
    }
}
