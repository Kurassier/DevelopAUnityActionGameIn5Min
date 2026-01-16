using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

//下面一行是开关，注释掉可以恢复原版Inspector
//[CustomEditor(typeof(AnimationClip)), CanEditMultipleObjects]
public class AnimationClipEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Button("123");
        Debug.Log(123);
    }
}
#endif