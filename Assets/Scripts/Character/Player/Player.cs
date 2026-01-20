using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public InputData input;
    [PropertyOrder(5)] public InputManager inputManager;
    [PropertyOrder(5)] public PlayerJump jumpComponent;
    [PropertyOrder(5)] public PlayerDash dashComponent;


    //平台穿越
    float platformPenetrateTimer;



    protected override void Awake()
    {
        base.Awake();

        //初始化
        input = new InputData();
        platformPenetrateTimer = -1;
    }


    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        inputManager.RefreshUpdate();

        jumpComponent.RefreshUpdate();
        dashComponent.RefreshUpdate();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        jumpComponent.RefreshFixedUpdate();
        moveComponent.RefreshFixedUpdate();
        dashComponent.RefreshFixedUpdate();

        RefreshPlatformPenetrate();
    }

    [ShowInInspector]
    public bool CanPenetratePlatform
    {
        get
        {
            return moveCollider.gameObject.layer == LayerMask.NameToLayer("CharacterIgnorePlatform");
        }
        set
        {
            if (value)
            {
                moveCollider.gameObject.layer = LayerMask.NameToLayer("CharacterIgnorePlatform");
            }
            else
            {
                moveCollider.gameObject.layer = LayerMask.NameToLayer("Character");
            }
        }
    }

    public void SetPlatformPenetrateTime(float time)
    {
        platformPenetrateTimer = time;
        CanPenetratePlatform = true;
    }

    //平台穿越
    void RefreshPlatformPenetrate()
    {
        bool canPenetrate = false;
        //强制穿越计时器，在计时器内允许穿透所有平台
        platformPenetrateTimer -= FixedFrameInterval;
        if (platformPenetrateTimer > 0)
            canPenetrate = true;
        //速度朝上时，允许穿透所有平台
        if (Rigidbody.velocity.y > 1f)
            canPenetrate = true;
        //Platform Sensor与平台相交时，始终允许穿越平台
        //（若Platform Sensor检测到平台，意味着前方而非下方出现平台，则可以穿越）
        if (characterState.isTouchingPlatform)
            canPenetrate = true;

        if (canPenetrate)
            CanPenetratePlatform = true;
        else
            CanPenetratePlatform = false;
    }


}
