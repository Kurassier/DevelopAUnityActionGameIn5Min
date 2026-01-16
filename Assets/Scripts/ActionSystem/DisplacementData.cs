using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
[CreateAssetMenu(fileName = "New Displace Prefab", menuName = "Datas/Displacement Prefab")]
public class DisplacementData : ScriptableObject
{
    public float maxSpeed;
    public float length;
    public AnimationCurve speedCurve;

    public DisplacementData()
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

    public static implicit operator Displacement  (DisplacementData displacementData)
    {
        return new Displacement(displacementData.maxSpeed, displacementData.length, displacementData.speedCurve);
    }
}

public class Displacement
{
    public float maxSpeed;
    public float length;
    public AnimationCurve speedCurve;

    public Displacement(float maxSpeed, float length, AnimationCurve speedCurve)
    {
        this.maxSpeed = maxSpeed;
        this.length = length;
        this.speedCurve = speedCurve;
    }
}