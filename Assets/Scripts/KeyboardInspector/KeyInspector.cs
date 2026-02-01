using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyInspector : MonoBehaviour
{
    public KeyCode key;

    SpriteRenderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        renderer.sprite = KeyboardReflection.Instance.GetKeySprite(key, Input.GetKey(key));
        renderer.color = Input.GetKey(key) ? new Color(1, 0.8f, 0.5f) : Color.white;
    }
}
