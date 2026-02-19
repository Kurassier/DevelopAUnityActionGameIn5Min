using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayerMaskPreset
{
    public static LayerMask DamageObstacle => LayerMask.GetMask("Ground", "Wall");
    public static LayerMask SightObstacle => LayerMask.GetMask("Ground", "Wall", "Platform");


}
