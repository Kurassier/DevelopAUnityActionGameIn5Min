using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateSensor : CharacterComponent
{
    #region obsolete
    //[ShowInInspector, PropertyOrder(2)]
    //public virtual bool IsOnGround { get => GroundCheck.IsTouchingLayers(LayerMaskDefault.GroundAndPlatform); }
    //[ShowInInspector, PropertyOrder(2)]
    //public bool IsFacingWall { get => WallCheck.IsTouchingLayers(LayerMaskDefault.GroundAndPlatform); }
    //[ShowInInspector, PropertyOrder(2)]
    //public bool IsFacingEdge
    //{
    //    get => !WallCheck.IsTouchingLayers(LayerMaskDefault.GroundAndPlatform)
    //        && !EdgeCheck.IsTouchingLayers(LayerMaskDefault.GroundAndPlatform)
    //        && GroundCheck.IsTouchingLayers(LayerMaskDefault.GroundAndPlatform);
    //}
    //[ShowInInspector, PropertyOrder(2)]
    //public virtual bool CantMoveForward => WallCheck.IsTouchingLayers(LayerMaskDefault.GroundAndPlatform) || !EdgeCheck.IsTouchingLayers(LayerMaskDefault.GroundAndPlatform);

    //public Collider2D GroundCheck
    //{
    //    get
    //    {
    //        if (groundCheck == null)
    //            groundCheck = transform.Find("Collide Checks").Find("Ground Check").GetComponent<Collider2D>();
    //        if (groundCheck == null)
    //            Debug.LogError("µØÃæ¼ì²âÆ÷²»´æÔÚ£¡");
    //        return groundCheck;
    //    }
    //}
    //public Collider2D WallCheck
    //{
    //    get
    //    {
    //        if (wallCheck == null)
    //            wallCheck = transform.Find("Collide Checks").Find("Wall Check").GetComponent<Collider2D>();
    //        if (wallCheck == null)
    //            Debug.LogError("Ç½¼ì²âÆ÷²»´æÔÚ£¡");
    //        return wallCheck;
    //    }
    //}
    //public Collider2D EdgeCheck
    //{
    //    get
    //    {
    //        if (edgeCheck == null)
    //            edgeCheck = transform.Find("Collide Checks").Find("Edge Check").GetComponent<Collider2D>();
    //        if (edgeCheck == null)
    //            Debug.LogError("±ßÔµ¼ì²âÆ÷²»´æÔÚ£¡");
    //        return edgeCheck;
    //    }
    //}
    //Collider2D groundCheck;
    //Collider2D wallCheck;
    //Collider2D edgeCheck;




    #endregion

    public Collider2D groundProbe;
    public Collider2D wallProbe;

    public override void RefreshFixedUpdate()
    {
        Owner.characterState.isFacingWall = Physics2D.IsTouchingLayers(wallProbe, LayerMask.GetMask("Ground"));
        Owner.characterState.isOnGround = Physics2D.IsTouchingLayers(groundProbe, LayerMask.GetMask("Ground", "Platform"));
    }



}

[System.Serializable]
public class CharacterState
{
    public bool isOnGround;
    public bool isFacingWall;
    public bool isTouchingPlatform;
}