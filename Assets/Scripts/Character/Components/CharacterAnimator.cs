using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : CharacterComponent
{
    public bool enableInterpolate = true;
    [SerializeField] Vector3 physicalPositionLastFrame = Vector3.zero;
    [SerializeField] float physicalIntervalTime = 0;
    [SerializeField] float offsetZ = 0;

    public override void Init()
    {
        base.Init();
        physicalIntervalTime = Time.fixedDeltaTime;
        physicalPositionLastFrame = Owner.RootPosition;
        offsetZ = transform.position.z;
    }

    public override void RefreshUpdate()
    {
        base.RefreshUpdate();

        // 帧间位置插值
        Vector3 physicalPosition = Owner.RootPosition;
        Vector3 physicalVelocity = (physicalPosition - physicalPositionLastFrame) / physicalIntervalTime;
        if (enableInterpolate)
            transform.position = physicalPositionLastFrame + physicalVelocity * TimeManager.Instance.TimeAfterLastFixedUpdate + new Vector3(0, 0, offsetZ);

    }

    public override void RefreshFixedUpdate()
    {
        base.RefreshFixedUpdate();

        // 帧间位置插值
        physicalIntervalTime = Time.fixedDeltaTime; // 玩家的FrameInterval还收到自身TimeScale影响，这里要使用整体的时间间隔

        physicalPositionLastFrame = Owner.RootPosition;
    }
}
