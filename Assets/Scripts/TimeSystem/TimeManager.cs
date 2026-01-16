using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TimeManager : Singleton<TimeManager>
{


    static List<Timer> timers;
    public static List<Timer> Timers
    {
        get
        {
            if (timers == null) timers = new List<Timer>();
            return timers;
        }
    }
    public static void ResetTimers()
    {
        timers = new List<Timer>();
    }

    [Sirenix.OdinInspector.ShowInInspector]
    int TimerCount { get => Timers.Count; }


    float globalTimeScale = 1;
    public static float GlobalTimeScale
    {
        get
        {
            return Instance.globalTimeScale;
        }
        set
        {
            Instance.globalTimeScale = Mathf.Clamp(value, 0.01f, 10f);
        }
    }

    double timeFromSceneBegun = 0;
    public static double TimeFromSceneBegun => TimeFromSceneBegun;


    public double timeFixedSinceLevelLoadAsDouble;
    public bool isLastFrameFixed = false;

    /// <summary>
    /// 只能在Update或LateUpdate中调用，返回当前帧与上一次Fixed或普通Update的时间间隔。
    /// </summary>
    public float IntervalBetweenUpdateOrFixed
    {
        get
        {
            if (isLastFrameFixed)
            {
                return (float)(Time.timeSinceLevelLoadAsDouble - timeFixedSinceLevelLoadAsDouble);
            }
            else
            {
                return Time.deltaTime;
            }
        }
    }

    public List<Timer> timerListForInspector;



    protected override void Awake()
    {
        base.Awake();

        timers = new List<Timer>();
        timerListForInspector = timers;
        Time.timeScale = 1;
    }
    // Update is called once per frame
    void Update()
    {
        //处理时间流速
        float timeScale = 1;
        if (timeScale > frameFreezeScale) timeScale = frameFreezeScale;
        if (timeScale > debugScale) timeScale = debugScale;
        if (timeScale > pauseScale) timeScale = pauseScale;
        if (timeScale > slowScale) timeScale = slowScale;
        Time.timeScale = timeScale;

        for (int i = timers.Count - 1; i >= 0; i--)
        {
            Timer timer = timers[i];
            //所有者不启用的计时器不更新
            if (timer.owner != null)
                if (!timer.owner.gameObject.activeInHierarchy)
                    continue;
            if (timer.type == TimerType.normal)
                if (timer.owner != null)
                    timer.Tick(timer.owner.FrameInterval);
                else
                    timer.Tick(Time.deltaTime);
            else if (timer.type == TimerType.unscaled)
                timer.Tick(Time.unscaledDeltaTime);
        }
        //ClearObsoleteTimers();

        isLastFrameFixed = false;
    }

    private void FixedUpdate()
    {
        for (int i = timers.Count - 1; i >= 0; i--)
        {
            Timer timer = timers[i];
            //所有者不启用的计时器不更新
            if (timer.owner != null)
                if (!timer.owner.gameObject.activeInHierarchy)
                    continue;

            if (timer.type == TimerType.fixedDelta)
                if (timer.owner != null)
                    timer.Tick(timer.owner.FixedFrameInterval);
                else
                    timer.Tick(Time.deltaTime);
            else if (timer.type == TimerType.fixedUnscale)
                timer.Tick(Time.fixedUnscaledDeltaTime);
        }
        //ClearObsoleteTimers();

        timeFromSceneBegun += Time.fixedTimeAsDouble;//每FixedUpdate帧增加一次ticks


        isLastFrameFixed = true;
        timeFixedSinceLevelLoadAsDouble = Time.timeSinceLevelLoadAsDouble;
    }


    public static void ResetScale()
    {
        Instance.frameFreezeScale = 1;
        Instance.pauseScale = 1;
        Instance.slowScale = 1;
        Instance.debugScale = 1;
    }

    [SerializeField] float frameFreezeScale = 1;
    [SerializeField] float pauseScale = 1;
    [SerializeField] float slowScale = 1;
    [SerializeField] float debugScale = 1;

    public static float SlowScale { get => Instance.slowScale; set => Instance.slowScale = value; }
    public static float DebugScale { get => Instance.debugScale; set => Instance.debugScale = value; }

    public static bool IsPause => Instance.pauseScale == 0;
    public static void Pause() => Instance.pauseScale = 0;
    public static void Unpause() => Instance.pauseScale = 1;

    //清除该销毁的计时器
    //void ClearObsoleteTimers()
    //{
    //    LinkedListNode<Timer> node = Timers.First;
    //    for (int i = 0; i < TimerCount; i++)
    //    {
    //        if (node.Value.needToDestroy)
    //        {
    //            LinkedListNode<Timer> next = node.Next;
    //            Timers.Remove(node);
    //            node = next;
    //        }
    //        else
    //        {
    //            node = node.Next;
    //        }
    //    }
    //}

    public static void FrameFreeze(float fullTime = 0.2f, float recoverTime = 0.1f, float minScale = 0.4f)
    {
        if (Instance.currentFrameFreezeCoroutine != null)
            Instance.StopCoroutine(Instance.currentFrameFreezeCoroutine);
        Instance.StartCoroutine(Instance.FrameFreezeCoroutine(fullTime, recoverTime, minScale));
    }

    Coroutine currentFrameFreezeCoroutine = null;
    IEnumerator FrameFreezeCoroutine(float fullTime = 0.2f, float recoverTime = 0.1f, float minScale = 0.4f)
    {
        for (float t = 0; t < fullTime; t += Time.unscaledDeltaTime)
        {
            if (t < fullTime - recoverTime)
            {
                frameFreezeScale = minScale;
            }
            else
            {
                frameFreezeScale = (t - fullTime + recoverTime) / recoverTime * (1 - minScale) + minScale;
            }
            yield return null;
        }
        frameFreezeScale = 1;
    }
}
