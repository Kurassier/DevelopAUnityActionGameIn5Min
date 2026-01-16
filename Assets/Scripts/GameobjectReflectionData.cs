using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Reflection Data", menuName = "Datas/ReflectionData")]
public class GameobjectReflectionData : ScriptableObject
{
    public string key = "None";
    public ReflectionItem[] reflections;

    //获取提供的key值对应的物体
    public GameObject GetValue(string key)
    {
        foreach(ReflectionItem item in reflections)
        {
            if (item.key == key)
                return item.value;
        }
        //如果没有找到对应的物体，返回null
        Debug.LogError("没有找到对应的物体映射关系，key为：" + key);
        return null;
    }

    //获取全部的物体映射关系数据
    static GameobjectReflectionData[] allReflectionData;
    public static GameobjectReflectionData[] AllReflectionData
    {
        get
        {
            if (allReflectionData == null)
            {
                allReflectionData = Resources.LoadAll<GameobjectReflectionData>("Reflections");
            }
            return allReflectionData;
        }
    }
    
    //获取特定的物体映射关系数据
    public static GameobjectReflectionData GetReflectionData(string key)
    {
        foreach(GameobjectReflectionData reflectionData in AllReflectionData)
        {
            if (reflectionData.key == key)
                return reflectionData;
        }
        return null;
    }
}

[System.Serializable]
public struct ReflectionItem
{
    [HorizontalGroup,HideLabel]
    public string key;
    [HorizontalGroup, HideLabel]
    public GameObject value;
}