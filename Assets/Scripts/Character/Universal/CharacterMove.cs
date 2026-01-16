using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMove : CharacterComponent
{
    protected bool IsForcedMoving => currentDisplacement != null;
    protected Displacement currentDisplacement = null;
    protected float currentDisplacementProgress = 0;

    public virtual Vector2 Velocity
    {
        get
        {
            return Owner.Rigidbody.velocity;
        }
        set
        {
            if (!IsForcedMoving)
                Owner.Rigidbody.velocity = value;
        }
    }
    public void SetForcedMove(Displacement displacement)
    {
        currentDisplacement = displacement;
        currentDisplacementProgress = 0;

    }

    // 每帧更新位移逻辑
    public override void RefreshFixedUpdate()
    {
        //强制移动
        if (IsForcedMoving)
        {
            Vector2 velocity = Owner.Velocity;
            currentDisplacementProgress += FixedFrameInterval;

            // 计算位移进度的比例
            float progressRate = currentDisplacementProgress / currentDisplacement.length;

            // 如果位移完成，清空当前位移并返回
            if (progressRate >= 1f)
            {
                currentDisplacement = null;
                return;
            }

            // 根据进度比例从速度曲线中获取速度因子
            float speedFactor = currentDisplacement.speedCurve.Evaluate(progressRate);
            // 更新刚体的水平速度
            velocity.x = Owner.Direction * currentDisplacement.maxSpeed * speedFactor;

            //一般来说，垂直速度都需要置为空
            velocity.y = 0;

            Owner.Rigidbody.velocity = velocity * TimeScale; // 应用新的速度
        }
    }
}
