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


    [SerializeField] float globalTimeScale = 1;
    [SerializeField] float frameFreezeScale = 1;
    [SerializeField] float pauseScale = 1;
    [SerializeField] float slowScale = 1;
    [SerializeField] float debugScale = 1;


    public static void ResetScale()
    {
        Instance.frameFreezeScale = 1;
        Instance.pauseScale = 1;
        Instance.slowScale = 1;
        Instance.debugScale = 1;
        Instance.globalTimeScale = 1;
    }
    public static float GlobleTimeScale
    {
        get
        {
            return Instance.globalTimeScale;
        }
    }
    public static float SlowScale
    {
        get => Instance.slowScale;
        set
        {
            Instance.slowScale = value;
            Instance.ResetGlobalScale();
        }
    }
    public static float DebugScale
    {
        get => Instance.debugScale;
        set
        {
            Instance.debugScale = value;
            Instance.ResetGlobalScale();
        }
    }

    double timeFromSceneBegun = 0;
    public static double TimeFromSceneBegun => TimeFromSceneBegun;


    public static double timeFixedSinceLevelLoadAsDouble;

    /// <summary>
    /// 只能在Update或LateUpdate中调用，返回当前帧与上一次FixedUpdate的时间间隔。
    /// </summary>
    public float TimeAfterLastFixedUpdate => (float)(Time.timeSinceLevelLoadAsDouble - timeFixedSinceLevelLoadAsDouble);

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
        ResetGlobalScale();

        //计时器
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

    }

    private void FixedUpdate()
    {
        //处理时间流速
        ResetGlobalScale();

        //计时器
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

        timeFixedSinceLevelLoadAsDouble = Time.timeSinceLevelLoadAsDouble;
    }

    void ResetGlobalScale()
    {
        float timeScale = 1;
        timeScale *= slowScale;
        timeScale *= pauseScale;
        timeScale *= frameFreezeScale;
        timeScale *= debugScale;
        Time.timeScale = timeScale;
        globalTimeScale = timeScale;
    }

    public static bool IsPause => Instance.pauseScale == 0;
    public static void Pause()
    {
        Instance.pauseScale = 0;
        Instance.ResetGlobalScale();
    }
    public static void Unpause()
    {
        Instance.pauseScale = 1;
        Instance.ResetGlobalScale();
    }

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

    public static void FrameFreeze(float fullTime)
    {
        if (Instance.currentFrameFreezeCoroutine != null)
            Instance.StopCoroutine(Instance.currentFrameFreezeCoroutine);
        Instance.currentFrameFreezeCoroutine = Instance.StartCoroutine(Instance.FrameFreezeCoroutine(fullTime));
    }

    Coroutine currentFrameFreezeCoroutine = null;
    IEnumerator FrameFreezeCoroutine(float fullTime)
    {
        for (float t = 0; t < fullTime; t += Time.unscaledDeltaTime)
        {
            frameFreezeScale = 0;
            ResetGlobalScale();
            yield return null;
        }
        frameFreezeScale = 1;
        ResetGlobalScale();
    }
}
