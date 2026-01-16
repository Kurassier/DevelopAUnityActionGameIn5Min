using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : PlayerComponent
{
    public const float MoveSpeed = 12f;
    public const float MoveAcceleration = 50f;
    public const float MoveBrake = 100f;

    public const float MoveAccelerationAirFactor = 0.2f;

    public override void RefreshFixedUpdate()
    {
        Vector2 velocity = Rigidbody.velocity;
        int moveInput = input.horizontalMove;

        //是否反向
        bool isReversing = moveInput * Owner.Direction < 0;
        if (isReversing && Owner.IsOnGround)
        {
            if(Mathf.Abs(velocity.x) > 3f)
                Owner.Animator.Play("RunTurn");
            else
                Owner.Animator.Play("RunStop");
        }

        //加速或减速，当当前移动方向与输入方向相同时，使用加速度，否则使用刹车加速度
        float acceleration = MoveBrake;
        if (moveInput * velocity.x > 0) 
            acceleration = MoveAcceleration;
        //如果在空中，则加速度乘以折损系数
        if(!Owner.IsOnGround) acceleration *= MoveAccelerationAirFactor;
        velocity.x = MathTools.MoveTo(velocity.x, moveInput * MoveSpeed, acceleration * FixedFrameInterval);

        //角色朝向
        if (moveInput != 0)
        {
            Owner.SetDirection(moveInput);
        }

        //动画
        bool isMove = Mathf.Abs(velocity.x) > 0.1f || moveInput != 0;
        Owner.Animator.SetBool("Is Move", isMove);

        Rigidbody.velocity = velocity;
    }
}
