using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMove : CharacterComponent
{
    [SerializeField, ReadOnly] protected Displacement currentDisplacement = null;
    [SerializeField, ReadOnly] protected float currentDisplacementTime = 0;


    public virtual Vector2 Velocity
    {
        get
        {
            return Owner.Rigidbody.velocity;
        }
        set
        {
            Owner.Rigidbody.velocity = value;
        }
    }
    protected bool IsForcedMoving => currentDisplacement != null;
    public void QuitForcedMoving() => currentDisplacement = null;
    public void SetForcedMove(Displacement displacement)
    {
        currentDisplacement = displacement;
        currentDisplacementTime = 0;
    }
    public virtual bool ForceMoveIsEqual(Displacement displacement)
    {
        return currentDisplacement == displacement;
    }

    // 每帧更新位移逻辑
    public override void RefreshFixedUpdate()
    {
        //强制移动
        if (IsForcedMoving)
        {
            Vector2 velocity = Velocity;
            currentDisplacementTime += FixedFrameInterval;

            // 计算位移进度的比例
            float progressRate = currentDisplacementTime / currentDisplacement.length;

            // 如果位移完成，清空当前位移并返回
            if (progressRate >= 1f)
            {
                currentDisplacement = null;
                return;
            }

            // 根据进度比例从速度曲线中获取速度因子
            float speedFactor = currentDisplacement.speedCurve.Evaluate(progressRate);
            // 更新刚体的水平速度
            velocity.x = Owner.Direction * currentDisplacement.maxSpeed * speedFactor * Owner.LocalTimeScale;

            //一般来说，强制移动时不应该受到重力影响（例如空中冲刺）
            velocity.y = 0;

            Velocity = velocity * TimeScale; // 应用新的速度
        }
    }
}
