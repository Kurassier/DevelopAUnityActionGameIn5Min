using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateSensor : CharacterStateSensor
{
    [SerializeField] Collider2D platformSensor;

    const float GroundLeaveTolerance = 0.2f;

    float groundLeaveTimer = -1;

    public override void RefreshFixedUpdate()
    {
        Owner.characterState.isFacingWall = wallProbe.IsTouchingLayers(LayerMask.GetMask("Ground"));

        //如果处于穿越平台的状态，地面检测不检测平台
        if (((Player)Owner).CanPenetratePlatform)
            Owner.characterState.isOnGround = groundProbe.IsTouchingLayers(LayerMask.GetMask("Ground"));
        else
            Owner.characterState.isOnGround = groundProbe.IsTouchingLayers(LayerMask.GetMask("Ground", "Platform"));

        //离地一段时间内，仍然判定为地面，因此如果当前在地面上，则设置宽容计时器。如果离地，宽容计时器自减
        if (Owner.characterState.isOnGround)
            groundLeaveTimer = GroundLeaveTolerance;
        else
            groundLeaveTimer -= FixedFrameInterval;
        //只要离地宽容计时器没有到期，玩家会一直被判定为地面状态
        Owner.characterState.isOnGround = groundLeaveTimer > 0f;


        Owner.characterState.isTouchingPlatform = platformSensor.IsTouchingLayers(LayerMask.GetMask("Platform"));
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        if (platformSensor != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(platformSensor.bounds.center, platformSensor.bounds.size);
        }
    }
}

