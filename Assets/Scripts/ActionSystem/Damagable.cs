using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iDamagable
{
    public HitResult Hit(Damage damage);

    public Faction Faction { get; }
    public bool HasLineOfSight(Vector2 target);
}


public class DamagableBox : MonoBehaviour,iDamagable
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

    public bool HasLineOfSight(Character target)
    {
        Vector2 direction;
        bool hasLOS = false;
        direction = target.ChestPosition - Position;
        hasLOS |= !Physics2D.Raycast(Position, direction.normalized, direction.magnitude,
            UnityEngine.LayerMask.GetMask("Ground", "Wall"));
        direction = target.ChestPosition + new Vector2(1, 0) - Position;
        hasLOS |= !Physics2D.Raycast(Position, direction.normalized, direction.magnitude,
            UnityEngine.LayerMask.GetMask("Ground", "Wall"));
        direction = target.ChestPosition + new Vector2(-1, 0) - Position;
        hasLOS |= !Physics2D.Raycast(Position, direction.normalized, direction.magnitude,
            UnityEngine.LayerMask.GetMask("Ground", "Wall"));

        return hasLOS;
    }
    public bool HasLineOfSight(Vector2 target)
    {
        Vector2 direction;
        bool hasLOS = false;
        direction = target - Position;
        hasLOS |= !Physics2D.Raycast(Position, direction.normalized, direction.magnitude,
            UnityEngine.LayerMask.GetMask("Ground", "Wall"));

        return hasLOS;
    }

    public float GetDistance(Character character)
    {
        return GetDistance(character.ChestPosition);
    }
    public float GetDistance(Vector2 target)
    {
        return (target - Position).magnitude;
    }
}