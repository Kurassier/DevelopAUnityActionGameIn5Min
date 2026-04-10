using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraShaker
    : Singleton<CameraShaker>
{
    public static float shakeFactor = 0.8f;


    //相机锁定计时器
    Timer CameraLockTimer;
    //当前相机的所有抖动效果
    List<ShakeInfo> shakeInfos;

    protected override void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        CameraLockTimer = new Timer(0, TimerType.normal);
        shakeInfos = new List<ShakeInfo>();
        shakeFactor = 1;
    }


    private void LateUpdate()
    {
        Vector2 offset = new Vector2();
        for (int i = shakeInfos.Count - 1; i >= 0; i--)
        {
            ShakeInfo info = shakeInfos[i];

            float progress = Mathf.Clamp01(info.t / info.time);
            float y = Mathf.Pow(Mathf.Abs(Mathf.Sin(Mathf.PI * info.repeat * progress)), 0.5f) * (1 - progress);
            offset += info.direction * info.magnitude * y;

            info.t += Time.unscaledDeltaTime;
            if(info.t > info.time)
                shakeInfos.RemoveAt(i);
        }
        shakeOffset = offset;
    }

    //相机抖动
    [SerializeField] Vector2 shakeOffset = new Vector2(0, 0);
    public static Vector2 ShakeOffset
    {
        //场景中不止存在一个虚拟相机，但是所有相机的抖动都统一从这里获取
        //此处仅计算偏移量，但是不具体应用到相机上
        get => Instance.shakeOffset * shakeFactor;
    }

    public static void CameraLock(float f)
    {
        if (Instance.CameraLockTimer < f)
            Instance.CameraLockTimer.Set(f);
    }


    public static void ShakeRandom(float magnitude, int repeat, float time)
    {
        //幅度、次数和时间都不能为0
        if (magnitude == 0 || repeat == 0 || time <= 0)
            return;
        float angle = Random.Range(0, 2 * Mathf.PI);
        Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        Shake(magnitude, repeat, time, dir);
    }
    public static void Shake(float magnitude, int repeat, float time, Vector2 dir)
    {
        //如果在锁定状态，不允许新加抖动，但是已有的抖动可以继续
        if (Instance.CameraLockTimer.InTime)
            return;
        //幅度、次数和时间都不能为0
        if (magnitude == 0 || repeat == 0 || time <= 0)
            return;
        //添加新的抖动信息
        ShakeInfo info = new ShakeInfo(magnitude, repeat, time, dir);
        Instance.shakeInfos.Add(info);
    }
}

public class ShakeInfo
{
    public float magnitude;
    public int repeat;
    public float time;
    public Vector2 direction;
    public float t;

    public ShakeInfo(float magnitude, int repeat, float time, Vector2 dir)
    {
        this.magnitude = magnitude;
        this.repeat = repeat;
        this.time = time;
        this.direction = dir;
        this.t = 0;
    }
}
