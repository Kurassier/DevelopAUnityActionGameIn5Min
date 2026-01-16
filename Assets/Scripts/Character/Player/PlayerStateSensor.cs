using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateSensor : CharacterStateSensor
{
    [SerializeField] Collider2D platformSensor;

    public override void RefreshFixedUpdate()
    {
        Owner.characterState.isFacingWall = wallProbe.IsTouchingLayers(LayerMask.GetMask("Ground"));

        if (((Player)Owner).CanPenetratePlatform)
            Owner.characterState.isOnGround = groundProbe.IsTouchingLayers(LayerMask.GetMask("Ground"));
        else
            Owner.characterState.isOnGround = groundProbe.IsTouchingLayers(LayerMask.GetMask("Ground", "Platform"));

        Owner.characterState.isTouchingPlatform = platformSensor.IsTouchingLayers(LayerMask.GetMask("Platform"));
    }
}

