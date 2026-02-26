using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : EnemyAction
{
    public GameObject attackHitbox = null;
    public float maxAttackRange = 2;        //向前的攻击距离
    public float maxAttackRangeBack = 1;    //向后的攻击距离
    public float attackFullTime = 1;        //攻击动作时长
    public float attackLastTracingTime = 0.25f;//攻击的最后一次追踪
    public float damage = 5;

    [ShowInInspector]
    Coroutine attackMeleeCorourine = null;
    Hitbox attackHitboxCurrrnt = null;

    float Distance => Owner.Direction * (Owner.TargetPosition.x - Owner.RootPosition.x);

    public override void Interrupt()
    {
        base.Interrupt();
        if (attackMeleeCorourine != null)
        {
            StopCoroutine(attackMeleeCorourine);
            attackMeleeCorourine = null;
        }
        if (attackHitboxCurrrnt != null) Destroy(attackHitboxCurrrnt.gameObject);
    }
    public override void StartAction(Action onActionEnd)
    {
        base.StartAction(onActionEnd);
        if (attackMeleeCorourine == null)
            attackMeleeCorourine = StartCoroutine(AttackMeleeCorourine());
    }

    protected IEnumerator AttackMeleeCorourine()
    {
        //先等待进入攻击范围
        while (Owner.Direction == 0 || (Distance > maxAttackRange || Distance < -maxAttackRangeBack) || Owner.TargetPosition.y - Owner.RootPosition.y > 2f)
        {
            if (Distance < -maxAttackRangeBack / 2)
            {
                Owner.Turn();
            }
            else
            {
                Owner.MoveState = EnemyMoveState.Chase;
            }

            //等待当前帧结束
            yield return new WaitForFixedUpdate();


            //丢失目标，则自动退出行动
            if(Owner.Target == null)
            {
                attackMeleeCorourine = null;
                actionEndCallback();
                yield break;
            }
        }

        //进行攻击
        {
            Debug.Log("Enemy Attack");

            //停止移动
            Owner.MoveState = EnemyMoveState.Hold;
            Owner.Velocity = Vector2.zero;

            //动作屏蔽
            Owner.AddIgnore(attackFullTime, ActionIgnoreTag.All);

            //动画播放
            Owner.Animator.Play("Attack", 0, 0);

            //生成攻击碰撞体
            attackHitboxCurrrnt = Hitbox.GenerateHitbox(attackHitbox, Owner, transform, damage, Owner.RootPosition);

            //最后一次追踪玩家位置
            yield return new WaitForSeconds(attackLastTracingTime);
            if (Distance < 0)
                Owner.ReverseDirection();

            //等待动作结束
            yield return new WaitForSeconds(attackFullTime - attackLastTracingTime);
        }

        attackMeleeCorourine = null;
        //回调函数
        actionEndCallback();

        Debug.Log("Attack End");
    }
}
