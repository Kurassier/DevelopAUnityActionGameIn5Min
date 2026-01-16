using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    static CameraShake main;
    static CameraShake Main
    {
        get
        {
            if (main == null)
            {
                main = new GameObject("CameraShake").AddComponent<CameraShake>();
                //初始晃动为0
                main.shakeOffset = Vector2.zero;
            }
            return main;
        }
    }

    public static float shakeFactor = 0.8f;


    //相机锁定计时器
    Timer CameraLockTimer;

    private void Start()
    {
        CameraLockTimer = new Timer(0, TimerType.normal);
        shakeFactor = 1;
    }


    //相机抖动
    [SerializeField] Vector2 shakeOffset = new Vector2(0, 0);
    public static Vector2 ShakeOffset
    {
        //场景中不止存在一个虚拟相机，但是所有相机的抖动都统一从这里获取
        //此处仅计算偏移量，但是不具体应用到相机上
        get => Main.shakeOffset * shakeFactor;
    }

    public static void CameraLock(float f)
    {
        if (Main.CameraLockTimer < f)
            Main.CameraLockTimer.Set(f);
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
    Coroutine shakeCoroutine = null;
    public static void Shake(float magnitude, int repeat, float time, Vector2 dir)
    {
        //幅度、次数和时间都不能为0
        if (magnitude == 0 || repeat == 0 || time <= 0)
            return;
        //检测是否正在启用
        if (Main.shakeCoroutine != null)
            Main.StopCoroutine(Main.shakeCoroutine);
        Main.StartCoroutine(Main.ShakeCoroutine(magnitude, repeat, time, dir));
    }

    IEnumerator ShakeCoroutine(float magnitude, int repeat, float time, Vector2 dir)
    {
        float y = 0;
        for (float t = 0; t < time; t += Time.unscaledDeltaTime)
        {
            y = Mathf.Pow(Mathf.Abs(Mathf.Sin(Mathf.PI * repeat * t / time)), 0.5f) * (1 - t / time);
            if (Mathf.Sin(2 * Mathf.PI * repeat * t / time) < 0) y *= -1;
            shakeOffset = dir * magnitude * y;
            yield return null;
        }
    }
}
