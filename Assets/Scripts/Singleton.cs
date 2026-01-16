using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    static T instance;
    public static T Instance => instance;

    protected virtual void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);

        instance = (T)this;
    }
}
