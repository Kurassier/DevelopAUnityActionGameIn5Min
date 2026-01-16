using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : PlayerComponent
{
    const float JumpCD = 0.3f;
    const float JumpSpeed = 20f;
    const float JumpDownSpeed = 20f;
    const float GravityUp = 60f;
    const float GravityFloat = 40f;
    const float GravityFall = 70f;
    const float MaxFallilngSpeed = 20;


    float jumpPreinput = -1f;
    float jumpDownPreinput = -1f;
    bool isOnGroundLastFrame = false;

    public override void RefreshUpdate()
    {
        jumpPreinput -= Time.unscaledDeltaTime;
        jumpDownPreinput -= Time.unscaledDeltaTime;
        if (input.jump)
        {
            jumpPreinput = 0.2f;
        }
        if (input.jumpDown)
        {
            jumpDownPreinput = 0.2f;
        }
    }

    public override void RefreshFixedUpdate()
    {
        VerticalMove();

        if (jumpPreinput > 0f && !Owner.IsIgnore(ActionIgnoreTag.Jump) && Owner.IsOnGround)
        {
            jumpPreinput = -1f;
            Jump();
        }

        if (jumpDownPreinput > 0f)
        {
            if (Owner.IsOnGround)
            {
                jumpDownPreinput = -1;
                PenetratePlatform();
            }
            else if (!Owner.IsIgnore(ActionIgnoreTag.Jump))
            {
                jumpDownPreinput = -1;
                JumpDown();
            }
        }
    }

    void Jump()
    {
        //动作屏蔽
        Owner.AddIgnore(JumpCD, ActionIgnoreTag.Jump);
        Owner.AddIgnore(0.2f, ActionIgnoreTag.All);

        //穿越平台
        Owner.SetPlatformPenetrateTime(0.1f);

        Vector2 velocity = Owner.Velocity;

        velocity.y = JumpSpeed;

        Owner.Velocity = velocity;
    }

    void JumpDown()
    {
        //动作屏蔽
        Owner.AddIgnore(JumpCD, ActionIgnoreTag.Jump);
        Owner.AddIgnore(0.2f, ActionIgnoreTag.All);

        //穿越平台
        Owner.SetPlatformPenetrateTime(0.13f);

        Vector2 velocity = Owner.Velocity;

        velocity.y = Mathf.Min(velocity.y, -JumpDownSpeed);

        Owner.Velocity = velocity;
    }

    void PenetratePlatform()
    {
        //穿越平台
        Owner.SetPlatformPenetrateTime(0.5f);
    }

    void VerticalMove()
    {       

        Vector2 velocity = Owner.Velocity;

        float gravity = 0;
        if (velocity.y > 2)
        {
            gravity = GravityUp;
        }
        else if (velocity.y > -2)
        {
            gravity = GravityFloat;
        }
        else
        {
            gravity = GravityFall;
        }
        velocity.y -= gravity * FixedFrameInterval;

        //限制最大下落速度
        velocity.y = Mathf.Max(velocity.y, -MaxFallilngSpeed);

        Owner.Velocity = velocity;

        //动画（浮空状态）
        Owner.Animator.SetBool("Is On Ground", Owner.IsOnGround);
        Owner.Animator.SetBool("Is Move Up", velocity.y > 0);

        //判断是否离地
        if (isOnGroundLastFrame && !Owner.IsOnGround)
        {
            if (velocity.y > 4)
            {
                Owner.Animator.Play("Jump Up", 0, 0);
            }
            else
            {
                Owner.Animator.Play("Jump Fall", 0, 0);
            }
        }


        //记录是否在地面
        isOnGroundLastFrame = Owner.IsOnGround;
    }
}
