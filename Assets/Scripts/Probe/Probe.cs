using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Probe : MonoBehaviour
{
    public UnityEngine.LayerMask layerMask;
    [ShowInInspector]
    public virtual bool IsContacted
    {
        get => false;
    }

}
