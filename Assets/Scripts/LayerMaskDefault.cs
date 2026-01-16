using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayerMaskDefault
{
    public static int Ground => LayerMask.GetMask("Ground");
    public static int GroundAndPlatform => LayerMask.GetMask("Ground", "Platform");
    public static int Character => LayerMask.GetMask("Character", "CharacterIgnorePlatform");

}
