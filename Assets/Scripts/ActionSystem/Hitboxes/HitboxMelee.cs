using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxMelee : Hitbox
{

    #region 常量数据
    //是否跟随发射者
    public bool isFollowOwner = false;
    //跟随时间
    public float followTime = -1;
    //生效前摇
    public float damagePoint = 0;
    //生效时长
    public float effectTime = 0.5f;
    #endregion

    //生效时间计时器
    float effectTimer = 0;
    //停止跟随计时器
    float endFollowTimer;
    //所有的子碰撞体，用于实现形状复杂的组合碰撞体
    Collider2D[] colliders;


    protected override void Init()
    {
        base.Init();
        colliders = GetComponentsInChildren<Collider2D>();
        //是否跟随发起者
        if (isFollowOwner)
            transform.parent = origin.transform;
        if (followTime > 0)
            endFollowTimer = followTime;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        //追随计时器
        endFollowTimer -= Time.fixedDeltaTime;
        if (endFollowTimer < 0)
            transform.parent = null;
        //开关碰撞体（无实际作用，仅方便查看合适伤害生效）
        foreach (Collider2D collider in colliders)
            collider.enabled = !(effectTimer < damagePoint || effectTimer > damagePoint + effectTime);
    }



    protected override List<iDamagable> CollideCheck()
    {
        List<iDamagable> hit = new List<iDamagable>();

        //生效时间检测
        effectTimer += Time.fixedDeltaTime;
        if (effectTimer < damagePoint || effectTimer > damagePoint + effectTime)
            return hit;

        List<Collider2D> result = new List<Collider2D>();
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

                    iDamagable damagable = collider.GetComponentInParent<iDamagable>();
                    if (damagable != null)
                    {
                        if (targetFaction.Contains(damagable.Faction))
                        {
                            hit.Add(damagable);
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
            if (!target.HitboxCenter.HasNoDamageObstacle(origin.ChestPosition)) continue;

            HitResult result = Hit(target, damage);
            //对所有敌人造成相同伤害
            //remainDamage -= result.damageAbsorb;
            if (result.hitResultType == HitResultType.Blocked)
            {
                type = HitResultType.Blocked;
                //关闭动画机显示
                GetComponentInChildren<Animator>().SetTrigger("Block");

                break;
            }
        }
    }
}
