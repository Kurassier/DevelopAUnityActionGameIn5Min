using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class CharacterComponent : MonoBehaviour
{
    Character owner;
    protected virtual Character Owner 
    {
        get
        {
            if (owner == null) owner = transform.GetComponentInParent<Character>();
            return owner;
        }
    }


    public float TimeScale => Owner.TimeScale;
    //帧间隔
    public float FixedFrameInterval => Owner.FixedFrameInterval;

    public float FrameInterval => Owner.FrameInterval;

    public virtual void Init() { }
    public virtual void RefreshUpdate() { }
    public virtual void RefreshFixedUpdate() { }
    public virtual void Interrupt() { }

}
