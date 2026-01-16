using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightRecorder : MonoBehaviour
{
    public float height = -9999;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y > height)
            height = transform.position.y;
    }
}
