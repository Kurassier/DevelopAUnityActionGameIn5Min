using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Keyboard Reflection", menuName = "Keyboard Reflection")]
public class KeyboardReflection : ScriptableObject
{
    [SerializeField] Sprite[] keysAlpha;
    [SerializeField] Sprite[] keysAlphaDown;
    [SerializeField] Sprite[] keysLetter;
    [SerializeField] Sprite[] keysLetterDown;

    [SerializeField] Sprite keyShift;
    [SerializeField] Sprite keyShiftDown;
    [SerializeField] Sprite keyCtrl;
    [SerializeField] Sprite keyCtrlDown;
    [SerializeField] Sprite keySpace;
    [SerializeField] Sprite keySpaceDown;
    [SerializeField] Sprite keyMouse0;
    [SerializeField] Sprite keyMouse0Down;
    [SerializeField] Sprite keyMouse1;
    [SerializeField] Sprite keyMouse1Down;

    static KeyboardReflection instance = null;
    public static KeyboardReflection Instance
    {
        get
        {
            if (instance == null)
                instance = Resources.Load<KeyboardReflection>("Keyboard Reflection");
            return instance;
        }
    }

    public Sprite GetKeySprite(KeyCode key, bool isDown)
    {
        if (key >= KeyCode.A && key <= KeyCode.Z)
        {
            int index = key - KeyCode.A;
            return isDown ? keysLetterDown[index] : keysLetter[index];
        }
        else if (key >= KeyCode.Alpha0 && key <= KeyCode.Alpha9)
        {
            int index = key - KeyCode.Alpha0;
            return isDown ? keysAlphaDown[index] : keysAlpha[index];
        }
        else if (key == KeyCode.LeftShift || key == KeyCode.RightShift)
        {
            return isDown ? keyShiftDown : keyShift;
        }
        else if (key == KeyCode.LeftControl || key == KeyCode.RightControl)
        {
            return isDown ? keyCtrlDown : keyCtrl;
        }
        else if (key == KeyCode.Space)
        {
            return isDown ? keySpaceDown : keySpace;
        }
        else if (key == KeyCode.Mouse0)
        {
            return isDown ? keyMouse0Down : keyMouse0;
        }
        else if (key == KeyCode.Mouse1)
        {
            return isDown ? keyMouse1Down : keyMouse1;
        }
        else
            return null;
    }
}
