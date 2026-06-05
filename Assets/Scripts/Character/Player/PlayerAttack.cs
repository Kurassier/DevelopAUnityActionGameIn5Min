using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : PlayerComponent
{
    public Displacement attackLightDisplacement;
    public Displacement attackHeavyDisplacement;


    const float attackHeavyChargeTime = 0.3f;
    const int attackComboCount = 2;

    public int attackCombo = 0;
    public float attackLightPreinput = -1;
    public float attackLightActionTimer = 0;
    public float attackHeavyChargeTimer = 0;



    public GameObject attackLightHitbox;
    public GameObject attackHeavyHitbox;

    Hitbox attackHitboxCurrrnt = null;

    public override void Init()
    {
        base.Init();
        attackCombo = 0;
        attackLightPreinput = -1;
        attackLightActionTimer = 0;
        attackHeavyChargeTimer = 0;
    }

    public override void RefreshFixedUpdate()
    {
        base.RefreshFixedUpdate();

        //攻击预输入比较特殊，按照游戏时长计算，不受时间速度影响，主要是为了做连段
        attackLightPreinput -= FixedFrameInterval;
        //攻击动作计时器，主要是为了在攻击动作中保存连段和重击蓄力时长
        attackLightActionTimer -= FixedFrameInterval;

        //角色地面和空中攻击需要完全分开
        // 角色在地面
        if (Owner.IsOnGround)
        {
            //如果没有屏蔽攻击动作
            if (!Owner.IsIgnore(ActionIgnoreTag.Action))
            {
                //轻击判定
                if (attackLightPreinput > 0)
                {
                    attackLightPreinput = 0;
                    AttackLight(attackCombo);
                    attackCombo++;
                }
                //轻击连段重置
                if (attackLightActionTimer <= 0)
                {
                    attackCombo = 0;
                }

                //蓄力重击计时
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

            }
            //如果屏蔽攻击动作（在其他动作中，也可能是攻击动作）
            else
            {
                //如果不在攻击动作中，重置连段和重击蓄力计时器
                if (attackLightActionTimer <= 0)
                {
                    attackCombo = 0;
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
            if (attackLightActionTimer > 0)
            {
                attackLightActionTimer = 0;
                if (Owner.ForceMoveIsEqual(attackLightDisplacement) || Owner.ForceMoveIsEqual(attackHeavyDisplacement))
                {
                    Owner.QuitForceMove();
                }
            }
            attackCombo = 0;
            attackHeavyChargeTimer = 0;
        }
    }
    public override void RefreshUpdate()
    {
        base.RefreshUpdate();

        if (input.attackLight)
        {
            attackLightPreinput = 0.2f;
            //攻击动作中的连段判定加长
            if (attackLightActionTimer > 0)
                attackLightPreinput = 0.5f;
        }
    }

    public override void Interrupt()
    {
        base.Interrupt();

        if (attackHitboxCurrrnt != null) Destroy(attackHitboxCurrrnt.gameObject);
    }

    void AttackLight(int combo)
    {
        //连段截断
        combo = combo % attackComboCount;

        //动作屏蔽
        Owner.AddIgnore(0.5f, ActionIgnoreTag.All);

        //动画播放
        string attackAnimName = "Attack L" + (combo + 1);
        Owner.Animator.Play(attackAnimName, 0, 0);

        //强制移动
        Owner.ForceMove(attackLightDisplacement);

        //启动攻击持续计时器，在此期间保存连段和重击蓄力时长
        attackLightActionTimer = 1.0f;

        //生成攻击碰撞体
        attackHitboxCurrrnt = Hitbox.GenerateHitbox(attackLightHitbox, Owner, transform, 5, Owner.RootPosition);

        //Debug.Log("Attack Light");
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


        //Debug.Log("Attack Heavy");
    }
}
