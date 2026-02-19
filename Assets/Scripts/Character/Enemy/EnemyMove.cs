using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class EnemyMove : CharacterMove
{
    protected new Enemy Owner => (Enemy)base.Owner;
    public float moveSpeed = 12f;
    public float moveAcceleration = 36f;
    public float turnTime = 0.25f;

    public float move = 0;
    public override void RefreshFixedUpdate()
    {
        //强制移动相关代码在CharacterMove中处理
        base.RefreshFixedUpdate();

        bool canMove = true;

        //判断是否有动作屏蔽
        if (Owner.IsIgnore(ActionIgnoreTag.Move))
            canMove = false;
        //AI的行为逻辑不同，强制移动永远优先于普通移动
        if (IsForcedMoving)
            canMove = false;
        //AI在空中完全无法行动
        if (!Owner.IsOnGround)
            canMove = false;

        //根据当前的移动状态，获取移动输入
        int moveInput = 0;
        if (Owner.MoveState != EnemyMoveState.Hold)
        {
            if (Owner.TargetPosition.x > Owner.RootPosition.x)
                moveInput = 1;
            else
                moveInput = -1;
            if (Owner.MoveState == EnemyMoveState.Flee)
                moveInput *= -1;
        }


        //获取当前速度
        Vector2 velocity = Velocity;

        //碰墙时不能继续朝墙的方向移动
        if (Owner.IsFacingWall && Owner.Direction * moveInput > 0)
            moveInput = 0;

        //是否反向，反向则执行转向动画，跳过所有后续动作
        bool isReversing = moveInput * Owner.Direction < -0;
        if (isReversing && canMove)
        {
            Turn();
            return;
        }

        if (velocity.x * moveInput < 0)
            velocity.x = 0;
        else
            velocity.x = Mathf.MoveTowards(velocity.x, moveInput * moveSpeed, FrameInterval * moveAcceleration);

        //动画
        bool isMove = Mathf.Abs(velocity.x) > 0.1f || moveInput != 0;
        Owner.Animator.SetBool("Is Move", isMove && canMove);

        if (canMove)
            Velocity = velocity;
    }

    public void Turn()
    {
        if (turnCoroutine == null && !Owner.IsIgnore(ActionIgnoreTag.Move))
            turnCoroutine = StartCoroutine(TurnCoroutine());
    }

    Coroutine turnCoroutine = null;
    IEnumerator TurnCoroutine()
    {
        int direction = Owner.Direction;

        //设置朝向并播放转向动画，但是转向都有延迟
        Owner.SetDirection(-direction); //先设置动画机朝向
        Owner.SetDirection(0);          //然后设置正在转向中
        Owner.Animator.Play("Turn", 0, 0);
        Owner.AddIgnore(turnTime, ActionIgnoreTag.Move);

        //等待转向结束
        yield return new WaitForSeconds(turnTime);
        Owner.SetDirection(-direction);

        turnCoroutine = null;
    }
}
