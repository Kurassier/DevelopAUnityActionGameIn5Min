using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : PlayerComponent
{
    public Displacement attackLightDisplacement;
    public Displacement attackHeavyDisplacement;

    public float attackPreinput = -1;
    public float attackHeavyChargeTimer = 0;
    const float attackHeavyChargeTime = 0.3f;

    public int attackLightCombo = 0;

    public float attackActionTimer = 0;

    public GameObject attackLightHitbox;
    public GameObject attackHeavyHitbox;

    Hitbox attackHitboxCurrrnt = null;

    public override void Init()
    {
        base.Init();
        attackPreinput = -1;
        attackHeavyChargeTimer = 0;
        attackLightCombo = 0;
        attackActionTimer = 0;
    }

    public override void RefreshFixedUpdate()
    {
        base.RefreshFixedUpdate();

        //攻击预输入比较特殊，按照游戏时长计算，不受时间速度影响，主要是为了做连段
        attackPreinput -= FixedFrameInterval;
        //攻击动作计时器，主要是为了在攻击动作中保存连段和重击蓄力时长
        attackActionTimer -= FixedFrameInterval;

        //角色地面和空中攻击需要完全分开
        // 角色在地面
        if (Owner.IsOnGround)
        {
            //如果没有屏蔽攻击动作
            if (!Owner.IsIgnore(ActionIgnoreTag.Action))
            {
                //轻击判定
                if (attackPreinput > 0)
                {
                    attackPreinput = 0;
                    AttackLight();
                }
                //重击蓄力计时
                if (input.attackHeavy)
                {
                    attackHeavyChargeTimer += FixedFrameInterval;
                    if (attackHeavyChargeTimer > attackHeavyChargeTime)
                    {
                        AttackHeavy();
                    }
                }
                else
                {
                    attackHeavyChargeTimer = 0;
                }
                //连段重置
                if (attackActionTimer <= 0)
                {
                    attackLightCombo = 0;
                }
            }
            //如果屏蔽攻击动作（在其他动作中，也可能是攻击动作）
            else
            {
                //如果不在攻击动作中，重置连段和重击蓄力计时器
                if (attackActionTimer <= 0)
                {
                    attackLightCombo = 0;
                    attackHeavyChargeTimer = 0;
                }
                else
                {
                    //重击蓄力计时
                    if (input.attackHeavy)
                        attackHeavyChargeTimer += FixedFrameInterval;
                    else
                        attackHeavyChargeTimer = 0;
                }
            }
        }
        // 角色在空中
        else
        {
            //在空中会打断攻击动画，并且退出攻击位移
            if (attackActionTimer > 0)
            {
                attackActionTimer = 0;
                if (Owner.ForceMoveIsEqual(attackLightDisplacement) || Owner.ForceMoveIsEqual(attackHeavyDisplacement))
                {
                    Owner.QuitForceMove();
                }
            }
            attackLightCombo = 0;
            attackHeavyChargeTimer = 0;
        }
    }
    public override void RefreshUpdate()
    {
        base.RefreshUpdate();

        if (input.attackLight)
        {
            attackPreinput = 0.2f;
            //攻击动作中的连段判定加长
            if (attackActionTimer > 0)
                attackPreinput = 0.5f;
        }
    }

    public override void Interrupt()
    {
        base.Interrupt();

        if (attackHitboxCurrrnt != null) Destroy(attackHitboxCurrrnt.gameObject);
    }

    void AttackLight()
    {
        //Debug.Log("Attack Light");

        //动作屏蔽
        Owner.AddIgnore(0.5f, ActionIgnoreTag.All);

        //动画播放
        string attackAnimName = "Attack L" + (attackLightCombo % 2 + 1);
        Owner.Animator.Play(attackAnimName, 0, 0);

        //强制移动
        Owner.ForceMove(attackLightDisplacement);

        //启动攻击持续计时器，在此期间保存连段和重击蓄力时长
        attackActionTimer = 1.0f;

        //生成攻击碰撞体
        attackHitboxCurrrnt = Hitbox.GenerateHitbox(attackLightHitbox, Owner, transform, 5, Owner.RootPosition);

        //连段增加
        attackLightCombo++;
    }

    void AttackHeavy()
    {
        //动作屏蔽
        Owner.AddIgnore(0.9f, ActionIgnoreTag.All);

        //动画播放
        string attackAnimName = "Attack H";
        Owner.Animator.Play(attackAnimName, 0, 0);

        //强制移动
        Owner.ForceMove(attackHeavyDisplacement);

        //生成攻击碰撞体
        attackHitboxCurrrnt = Hitbox.GenerateHitbox(attackHeavyHitbox, Owner, transform, 5, Owner.RootPosition);
    }
}
