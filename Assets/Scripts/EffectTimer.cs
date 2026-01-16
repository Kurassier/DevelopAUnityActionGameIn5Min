using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTimer : MonoBehaviour
{
    public float time = 0.5f;

    private void Update()
    {
        time -= Time.deltaTime;
        if (time < 0)
        {
            Destroy(gameObject);
        }
    }
}
