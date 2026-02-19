using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iDamagable
{
    public HitResult Hit(Damage damage);

    public Faction Faction { get; }
    public Vector2 HitboxCenter { get; }
}


public class DamagableBox : MonoBehaviour, iDamagable
{
    public Vector2 Position => transform.position;

    public HitResult Hit(Damage damage)
    {
        return null;
    }

    public Faction Faction
    {
        get;
    }

    public Vector2 HitboxCenter { get => Position; }

    public float GetDistance(Character character)
    {
        return GetDistance(character.ChestPosition);
    }
    public float GetDistance(Vector2 target)
    {
        return (target - Position).magnitude;
    }
}