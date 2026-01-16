using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : PlayerComponent
{
    const float JumpCD = 0.7f;
    const float JumpSpeed = 20f;
    const float GravityUp = 60f;
    const float GravityFloat = 40f;
    const float GravityFall = 70f;


    float jumpPreinput = -1f;
    bool isOnGroundLastFrame = false;

    public override void RefreshUpdate()
    {
        jumpPreinput -= Time.unscaledDeltaTime;
        if (input.jump)
        {
            jumpPreinput = 0.2f;
        }
    }

    public override void RefreshFixedUpdate()
    {
        VerticalMove();

        if (jumpPreinput > 0f && !Owner.IsIgnore(ActionIgnoreTag.Jump) && Owner.IsOnGround)
        {
            Jump();
        }
    }

    void Jump()
    {
        //添加屏蔽
        Owner.AddIgnore(JumpCD, ActionIgnoreTag.Jump);

        //穿越平台
        Owner.SetPlatformPenetrateTime(0.1f);

        Vector2 velocity = Rigidbody.velocity;

        velocity.y = JumpSpeed;

        Rigidbody.velocity = velocity;
    }

    void VerticalMove()
    {

        Vector2 velocity = Rigidbody.velocity;

        float gravity = 0;
        if(velocity.y > 2)
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

        Rigidbody.velocity = velocity;

        //动画（浮空状态）
        Owner.Animator.SetBool("Is On Ground", Owner.IsOnGround);
        Owner.Animator.SetBool("Is Move Up", velocity.y > 0);

        //判断是否离地
        if(isOnGroundLastFrame && !Owner.IsOnGround)
        {
            if(velocity.y > 4)
            {
                Owner.Animator.Play("Jump Up");
            }
            else
            {
                Owner.Animator.Play("Jump Fall");
            }
        }


        //记录是否在地面
        isOnGroundLastFrame = Owner.IsOnGround;
    }
}
