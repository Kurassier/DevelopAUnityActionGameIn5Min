using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Faction { player, enemy, ally, friendly, neutral, all }
public static class FactionExtension
{
    //判断target是否属于origin，一般用于判断目标是否可被选中
    public static bool Contains(this Faction origin, Faction target)
    {
        if (origin == target) return true;
        if (origin == Faction.all) return true;
        if(origin == Faction.friendly)
        {
            if (target == Faction.player) return true;
            if (target == Faction.ally) return true;
        }
        //中立可以被任何组（除了player以外）选中
        if (origin != Faction.player && target == Faction.neutral) return true;
        return false;
    }

    //判断target是否与origin敌对，不可用于判断freindly与all（friendly与player友好，但与enemy敌对，无正确结果）
    public static bool Hostile(this Faction origin, Faction target)
    {
        if (origin == target) return false;
        if (origin == Faction.ally)
            if (target == Faction.player) return false;
        if (origin == Faction.player)
            if (target == Faction.ally) return false;

        return true;
    }


    //获取该阵营的敌对阵营（如果输入All，则返回All）
    public static Faction GetHostileFaction(this Faction origin)
    {
        if (origin == Faction.player) return Faction.enemy;
        if (origin == Faction.ally) return Faction.enemy;
        if (origin == Faction.friendly) return Faction.enemy;
        if (origin == Faction.enemy) return Faction.friendly;
        return Faction.all;
    }


}
