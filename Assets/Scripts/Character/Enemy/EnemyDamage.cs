using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : CharacterDamage
{
    public GameObject hitSpark;
    public override HitResult Hit(Damage damage)
    {
        Debug.Log("Enemy Hit");
        Vector2 randomOffset = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)) * 0.25f;
        Quaternion randomRotation = Quaternion.Euler(0, 0, Random.Range(-20, 20));
        GameObject.Instantiate(hitSpark, Owner.ChestPosition + randomOffset, randomRotation, Owner.transform);
        return base.Hit(damage);
    }
}
