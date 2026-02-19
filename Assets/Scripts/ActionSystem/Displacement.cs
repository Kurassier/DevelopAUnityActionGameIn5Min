using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
[CreateAssetMenu(fileName = "New Displace Prefab", menuName = "Datas/Displacement Prefab")]
public class Displacement : ScriptableObject
{
    public float maxSpeed;
    public float length;
    public AnimationCurve speedCurve;

    public Displacement()
    {
        maxSpeed = 5;
        length = 1;
        speedCurve = AnimationCurve.Linear(0, 1, 1, 0);
    }

    [Button("Reset")]
    public void Reset()
    {
        maxSpeed = 5;
        length = 1;
        speedCurve = AnimationCurve.Linear(0, 1, 1, 0);
    }
}