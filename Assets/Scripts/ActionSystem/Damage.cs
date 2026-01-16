using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ImpulseType { Parallel, Centrifugal }
public enum DamageType { Melee, Missle, Explosion }
public enum ImpactType { None, Shot, Blunt, Light, Heavy, Explosive, Devastated }

public class Damage
{

    public float damage = 0;
    public Character origin = null;

    public DamageType damageType = DamageType.Melee;

    public float impulse = 0;
    public ImpulseType impulseType = ImpulseType.Parallel;
    //根据冲击类型，平行冲击表示方向，向心冲击表示中心位置
    public Vector2 impulseVector = new Vector2();
    public ImpactType impact = ImpactType.None;


    public Damage(float damage, Character origin, DamageType damageType, ImpactType impact)
    {
        this.damage = damage;
        this.origin = origin;
        this.damageType = damageType;
        this.impulse = 0;
        this.impulseType = ImpulseType.Parallel;
        this.impulseVector = new Vector2();
        this.impact = impact;
    }
    public Damage(float damage, Character origin, DamageType damageType,
        float impulse, Vector2 impulseVector, ImpulseType impulseType = ImpulseType.Parallel, ImpactType impact = ImpactType.None)
    {
        this.damage = damage;
        this.origin = origin;
        this.damageType = damageType;
        this.impulse = impulse;
        this.impulseType = impulseType;
        this.impulseVector = impulseVector;
        this.impact = impact;
    }

    public Vector2 GetImpulse(Vector2 victim)
    {
        if (impulseType == ImpulseType.Parallel)
            return impulseVector.normalized * impulse;
        else
        {
            return (victim - impulseVector.normalized).normalized * impulse;
        }
    }

}