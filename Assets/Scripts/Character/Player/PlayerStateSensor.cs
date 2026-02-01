using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateSensor : CharacterStateSensor
{
    [SerializeField] Collider2D platformSensor;


    public override void RefreshFixedUpdate()
    {
        Owner.characterState.isFacingWall = wallProbe.IsTouchingLayers(LayerMask.GetMask("Ground"));

        //如果处于穿越平台的状态，地面检测不检测平台
        if (((Player)Owner).CanPenetratePlatform)
            Owner.characterState.isOnGround = groundProbe.IsTouchingLayers(LayerMask.GetMask("Ground"));
        else
            Owner.characterState.isOnGround = groundProbe.IsTouchingLayers(LayerMask.GetMask("Ground", "Platform"));

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

