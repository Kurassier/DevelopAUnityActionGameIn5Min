using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum ActionIgnoreTag { Move, Attack, Dash, Jump, WallSlide, Interact, All = 63 }

[System.Serializable]
public struct ActionIgnoreMask
{
    int maskValue;

    public static ActionIgnoreMask GetMask(params ActionIgnoreTag[] actionIgnores)
    {
        ActionIgnoreMask mask = new ActionIgnoreMask();
        mask.maskValue = 0;
        foreach (ActionIgnoreTag tag in actionIgnores)
        {
            if (tag == ActionIgnoreTag.All)
            {
                mask.maskValue = int.MaxValue;
                return mask;
            }
            else
            {
                int value = 1 << (int)tag;
                mask.maskValue |= value;
            }
        }
        return mask;
    }
    public bool ContainTag(ActionIgnoreTag tag)
    {
        return (maskValue >> (int)tag) % 2 == 1;
    }


    public static bool operator ==(ActionIgnoreMask mask1, ActionIgnoreMask mask2)
    {
        return mask1.maskValue == mask2.maskValue;
    }
    public static bool operator !=(ActionIgnoreMask mask1, ActionIgnoreMask mask2)
    {
        return mask1.maskValue != mask2.maskValue;
    }
}

[System.Serializable]
public class ActionIgnore
{
    public ActionIgnoreMask mask;
    [PropertyOrder(1)]
    public float timer;

    [ShowInInspector, PropertyOrder(0)]
    public string MaskToString
    {
        get
        {
            string s = "";
            foreach (ActionIgnoreTag actionIgnore in AllTags)
            {
                if (mask.ContainTag(actionIgnore))
                    s += actionIgnore.ToString() + " ";
            }
            if (mask.ContainTag(ActionIgnoreTag.All))
                s = "ALL";
            return s;
        }
    }

    public ActionIgnore(ActionIgnoreMask mask, float time)
    {
        this.mask = mask;
        timer = time;
    }

    public static ActionIgnoreTag[] AllTags
    {
        get
        {
            ActionIgnoreTag[] list = new ActionIgnoreTag[] {
                ActionIgnoreTag.Move, ActionIgnoreTag .Attack, ActionIgnoreTag .Dash, ActionIgnoreTag .Jump, ActionIgnoreTag.Interact};
            return list;
        }
    }
}