using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager main;

    List<Character> allCharacters;
    public static List<Character> AllCharacters => main.allCharacters;
    public event System.Action<Character> UnitDestroyEvent;

    int totalKill = 0;
    public static int TotalKill => main.totalKill;

    int enemyRemain = 0;
    public static int EnemyRemain => main.enemyRemain;

    private void Awake()
    {
        if (main != null) Destroy(main.gameObject);
        main = this;

        allCharacters = new List<Character>();

        UnitDestroyEvent = null;

        totalKill = 0;
        enemyRemain = 0;
    }

    private void FixedUpdate()
    {

    }

    public static void AddUnit(Character character)
    {
        if (!main.allCharacters.Contains(character))
        {
            main.allCharacters.Add(character);
            if(character.Faction == Faction.enemy)
            {
                main.enemyRemain++;
            }
        }
    }

    public static void RemoveUnit(Character victim)
    {
        //从列表中移除角色
        if (main.allCharacters.Remove(victim))
        {
            //计数器
            if (victim.Faction == Faction.enemy)
                main.enemyRemain--;
            //触发击杀事件
            if (main.UnitDestroyEvent != null)
                main.UnitDestroyEvent(victim);

            main.totalKill++;
        }
    }
}
