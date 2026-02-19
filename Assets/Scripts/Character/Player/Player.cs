using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public static Player Instance { get; private set; }

    public InputData input;
    [PropertyOrder(5)] public InputManager inputManager;
    [PropertyOrder(5)] public PlayerJump jumpComponent;
    [PropertyOrder(5)] public PlayerDash dashComponent;
    [PropertyOrder(5)] public PlayerAttack attackComponent;





    protected override void Awake()
    {
        base.Awake();

        if (Instance != null)
            Destroy(Instance.gameObject);
        Instance = this;

        //初始化
        input = new InputData();
        platformPenetrateTimer = -1;
    }


    protected override void Start()
    {
        base.Start();

        moveComponent.Init();
        jumpComponent.Init();
        dashComponent.Init();
        attackComponent.Init();
    }

    protected override void Update()
    {
        base.Update();

        inputManager.RefreshUpdate();

        jumpComponent.RefreshUpdate();
        dashComponent.RefreshUpdate();
        attackComponent.RefreshUpdate();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        jumpComponent.RefreshFixedUpdate();
        moveComponent.RefreshFixedUpdate();
        dashComponent.RefreshFixedUpdate();
        attackComponent.RefreshFixedUpdate();

        RefreshPlatformPenetrate();
    }
    public override void Interrupt()
    {
        base.Interrupt();
    }

    [ShowInInspector]
    public bool CanPenetratePlatform
    {
        get
        {
            return moveCollider.gameObject.layer == LayerMask.NameToLayer("CharacterIgnorePlatform");
        }
        protected set
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

    //平台穿越
    float platformPenetrateTimer;
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
        if (characterState.isTouchingPlatform)
            canPenetrate = true;

        if (canPenetrate)
            CanPenetratePlatform = true;
        else
            CanPenetratePlatform = false;
    }


}
