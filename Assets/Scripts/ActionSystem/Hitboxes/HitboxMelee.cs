using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxMelee : Hitbox
{
    //停止跟随计时器
    Timer endFollowTimer;
    //所有的子碰撞体，用于实现形状复杂的组合碰撞体
    Collider2D[] colliders;


    private void Start()
    {
        colliders = GetComponentsInChildren<Collider2D>();
        if (followTime > 0)
            endFollowTimer = new Timer(followTime, TimerType.fixedDelta, null, EndAttach);
    }

    void EndAttach()
    {
        transform.parent = null;
    }

    protected virtual List<Character> CollideCheck()
    {
        List<Collider2D> result = new List<Collider2D>();
        List<Character> hit = new List<Character>();
        ContactFilter2D filter = new ContactFilter2D();
        //filter.SetLayerMask(LayerMask.GetMask("Character", "Ground", "Wall"));
        //对所属的每一个碰撞体都要做检测
        foreach (Collider2D childCollider in colliders)
        {
            if (!childCollider.isTrigger) continue;
            if (!childCollider.enabled) continue;

            int count = childCollider.OverlapCollider(filter, result);
            foreach (Collider2D collider in result)
            {
                //已经结算过碰撞的碰撞体不再重新结算
                if (!touchedColliders.Contains(collider))
                {
                    //判断是否属于某个角色

                    Character character = collider.GetComponentInParent<Character>();
                    if (character != null)
                    {
                        if (targetFaction.Contains(character.Faction))
                        {
                            hit.Add(character);
                        }
                    }

                    touchedColliders.Add(collider);
                }
            }
        }
        return hit;
    }
    protected override HitResult Hit(iDamagable target, float remainDamage)
    {
        HitResult result = base.Hit(target, remainDamage);

        //命中特效只播一次
        if (!isHit && result.hitResultType != HitResultType.Miss)
        {
            PlayHitEffect();
            isHit = true;
        }
        return result;
    }
    protected override void HitResultCheck(List<iDamagable> hitTargets)
    {

        //依次结算命中
        HitResultType type = HitResultType.Stucked;
        //
        //hitTargets.Sort((x, y) => (x.ChestPosition - (Vector2)transform.position).magnitude.CompareTo((y.ChestPosition - (Vector2)transform.position).magnitude));
        foreach (iDamagable target in hitTargets)
        {
            //近战需要检测LOS，若中间被Wall或Ground挡住，则不算命中
            //if (!target.HasLineOfSight(origin)) continue;

            HitResult result = Hit(target, remainDamage);
            remainDamage -= result;
            if (result.hitResultType == HitResultType.Blocked)
            {
                type = HitResultType.Blocked;
                //关闭动画机显示
                GetComponentInChildren<Animator>().SetTrigger("Block");

                break;
            }
            if (remainDamage <= 0)
            {
                type = HitResultType.Stucked;
                break;
            }
        }
        //如果消耗完了所有剩余伤害
        if (remainDamage <= 0)
        {
            Stucked(type);
        }
    }
}
