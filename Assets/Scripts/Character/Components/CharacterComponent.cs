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
    protected virtual Rigidbody2D Rigidbody => Owner.Rigidbody;


    public float TimeScale => Owner.TimeScale;
    //Ö¡¼ä¸ô
    public float FixedFrameInterval => Owner.FixedFrameInterval;

    public float FrameInterval => Owner.FrameInterval;

    public virtual void Init() { }
    public virtual void RefreshUpdate() { }
    public virtual void RefreshFixedUpdate() { }


}
