using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxExplosion : Hitbox
{
    public string sound = "Enemy_Grenade_Explode";

    protected override void Awake()
    {
        base.Awake();
    }

    protected override List<iDamagable> CollideCheck()
    {
        List<iDamagable> hit = base.CollideCheck();
        for (int i = hit.Count - 1; i >= 0; i--)
        {
            if (!hit[i].HasLineOfSight(transform.position))
            {
                hit.RemoveAt(i);
            }
        }
        return hit;
    }

    protected override void HitResultCheck(List<iDamagable> hitTargets)
    {
        //爆炸没有伤害上限       
        //remainDamage 此时表示伤害值

        //爆炸碰撞箱无论命中与否，都会触发震动特效
        //碰撞检查每帧都会触发，但是只有第一次会触发命中效果
        if (isHit == false)
            PlayHitEffect();
        isHit = true;

        //依次结算命中
        HitResultType type = HitResultType.Stucked;
        foreach (iDamagable target in hitTargets)
        {
            HitResult result = Hit(target, remainDamage);
            //爆炸伤害可以被格挡抵消，但是不能保护身后单位，也不会弹刀
            if (result.hitResultType == HitResultType.Blocked)
            {
                type = HitResultType.Blocked;
            }
        }

        //爆炸伤害永远不会卡刀

    }


    public override void Destroy()
    {
        Destroy(this);
    }
}
