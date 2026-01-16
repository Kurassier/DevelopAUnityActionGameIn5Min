using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    List<double> frames;
    public float interval = 0.25f;

    UnityEngine.UI.Text text;


    // Start is called before the first frame update
    void Start()
    {
        frames = new List<double>();
        text = GetComponent<UnityEngine.UI.Text>();

        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        frames.Add(Time.realtimeSinceStartupAsDouble);
        while (Time.realtimeSinceStartupAsDouble - frames[0] > interval)
            frames.RemoveAt(0);

        text.text = "FPS: " + (int)(frames.Count / interval);

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (TimeManager.DebugScale > 0.5f)
                TimeManager.DebugScale = 0.1f;
            else
                TimeManager.DebugScale = 1;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            //UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
        if (Input.GetKeyDown(KeyCode.LeftAlt))
            UnityEditor.EditorApplication.isPaused = true;
#endif 
    }
}
