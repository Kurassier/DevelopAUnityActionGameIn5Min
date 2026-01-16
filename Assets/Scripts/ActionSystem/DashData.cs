using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dash", menuName = "Custom/Dash Data")] 
public class DashData : ScriptableObject
{
    public float length;
    public float maxSpeed;
    public AnimationCurve curve;

    public float GetSpeed(float time)
    {
        if (length < time)
            return 0;
        else
            return curve.Evaluate(time) * maxSpeed;
    }

    public DashData Clone()
    {
        var clone = new DashData();
        clone.maxSpeed = maxSpeed;
        clone.length = length; 
        clone.curve = curve;
        return clone;
    }
}
