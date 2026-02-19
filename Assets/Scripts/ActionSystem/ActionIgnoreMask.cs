using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

public enum ActionIgnoreTag { Move, Action, Dash, Jump, Interact, All = 31 }

[System.Serializable]
public struct ActionIgnoreMask
{
    int maskValue;

    public static ActionIgnoreMask GetMask(params ActionIgnoreTag[] actionIgnores)
    {
        ActionIgnoreMask mask = new ActionIgnoreMask();
        mask.maskValue = 0;
        if (actionIgnores.Contains(ActionIgnoreTag.All))
        {
            mask.maskValue = (int)ActionIgnoreTag.All;
            return mask;
        }
        foreach (ActionIgnoreTag tag in actionIgnores)
        {
            int value = 1 << (int)tag;
            mask.maskValue |= value;
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
    public float timer;

    public ActionIgnore(ActionIgnoreMask mask, float time)
    {
        this.mask = mask;
        timer = time;
    }

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
            if (mask == ActionIgnoreMask.GetMask(ActionIgnoreTag.All))
                s = "ALL";
            return s;
        }
    }


    public static ActionIgnoreTag[] AllTags
    {
        get
        {
            ActionIgnoreTag[] list = new ActionIgnoreTag[] {
                ActionIgnoreTag.Move, ActionIgnoreTag .Action, ActionIgnoreTag .Dash, ActionIgnoreTag .Jump,  ActionIgnoreTag.Interact};
            return list;
        }
    }
}