using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDamage : CharacterComponent
{
    public Displacement hitRepel;
    public virtual HitResult Hit(Damage damage)
    {
        switch (damage.impact)
        {
            case ImpactType.None:
                break;
            default:
                //打断所有行动
                Owner.Interrupt();

                //获取冲击方向
                Vector2 direction = damage.impulseVector;
                if (damage.impulseType == ImpulseType.Centrifugal)
                    direction = Owner.ChestPosition - damage.impulseVector;
                //修改朝向，然后被击退
                Owner.SetDirection(-direction.x);
                Owner.ForceMove(hitRepel);
                //播放动画，设置动作忽略
                Owner.Animator.SetTrigger("Hit");
                Owner.AddIgnore(hitRepel.length, ActionIgnoreTag.All);
                break;
        }

        return new HitResult(damage.damage, HitResultType.Hit);
    }

    public virtual void Dead()
    {

    }
}
