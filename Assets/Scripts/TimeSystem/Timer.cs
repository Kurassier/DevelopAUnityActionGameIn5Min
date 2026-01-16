using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum TimerType { normal, unscaled, fixedDelta, fixedUnscale }

public delegate void TimerEvent();

[System.Serializable]
public class Timer
{
    [Sirenix.OdinInspector.ShowInInspector]
    float remainTime = 0;
    //是否是一次性的
    public bool disposable = false;
    //是否应当被删除
    public bool needToDestroy = false;
    public TimerType type = TimerType.normal;
    public TimerEvent TimerTimeOut;
    public TimerEvent TimerTick;
    //是否受某个单位的时间流速影响
    public Character owner = null;

    //可重用的Timer，time不重要，重要的是owner
    public Timer(Character owner = null, TimerType type = TimerType.fixedDelta, TimerEvent timeOutEvent = null,
         TimerEvent tickEvent = null, bool disposable = false, float time = -1)
    {
        remainTime = time;
        this.type = type;
        this.disposable = disposable;
        TimerTimeOut = timeOutEvent;
        TimerTick = tickEvent;
        this.owner = owner;

        TimeManager.Timers.Add(this);

    }

    //经常是一次性的Timer，往往没有owner，time和event更重要
    public Timer(float time, TimerType type = TimerType.fixedDelta, Character owner = null, TimerEvent timeOutEvent = null,
         TimerEvent tickEvent = null, bool disposable = false)
    {
        remainTime = time;
        this.type = type;
        this.disposable = disposable;
        TimerTimeOut = timeOutEvent;
        TimerTick = tickEvent;
        this.owner = owner;

        TimeManager.Timers.Add(this);
    }

    public void SetOwner(Character owner) => this.owner = owner;

    public void Tick(float time)
    {
        //记录时间流逝前是否到期
        bool inTime = remainTime > 0;

        remainTime -= time;

        //触发每帧Tick事件
        if (TimerTick != null)
            TimerTick();

        //若时间流逝后剩余时间小于0，触发计时器到期时间
        if (inTime && remainTime <= 0)
            if (TimerTimeOut != null)
                TimerTimeOut();

        //若计时器是一次性的
        if (disposable && remainTime < 0)
            Destroy();
    }

    //重新设置计时器时间
    public void Set(float t) => remainTime = t;
    //取消计时器，不会触发到期
    public void Cancel() => remainTime = -1;
    //销毁计时器
    public void Destroy() => needToDestroy = true;

    //计时器在时间内
    public bool InTime { get => remainTime > 0; }
    //计时器已到期
    public bool TimeOut { get => remainTime <= 0; }

    public float Time => remainTime;

    public static implicit operator float(Timer timer)
    {
        return timer.remainTime;
    }

    public static Timer operator -(Timer timer, float deltaTime)
    {
        timer.remainTime -= deltaTime;
        return timer;
    }

}
