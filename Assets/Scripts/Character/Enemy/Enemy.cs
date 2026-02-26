using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyMoveState { Chase, Hold, Flee }
public class Enemy : Character
{
    //————————敌人AI相关变量————————
    [ShowInInspector, ReadOnly] public Character Target;
    [ShowInInspector, ReadOnly] public Vector2 TargetPosition;
    [ShowInInspector, ReadOnly] public EnemyMoveState MoveState;
    //等待计时器，用于让敌人短暂发呆，给动作一些衔接空间
    public Timer waitTimer;
    //动作列表，会随机从中抽取一个作为下一个动作
    public EnemyAction[] actions = new EnemyAction[0];
    public EnemyAction actionCurrent = null;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        waitTimer = new Timer(this);
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        //行动AI
        if (waitTimer.TimeOut && !IsIgnore(ActionIgnoreTag.Action)) 
        {
            //索敌
            Searching();

            if (actionCurrent == null && Target != null)
            {
                //选择动作
                SelectAction();
            }
            else if (Target == null)
            {
                MoveState = EnemyMoveState.Hold;
            }

        }

        //移动
        moveComponent.RefreshFixedUpdate();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
    }
    public override void Interrupt()
    {
        base.Interrupt();
        ActionEnd();
        waitTimer.Set(0.5f);
    }

    //最基础的索敌逻辑
    void Searching()
    {
        Character target = Player.Instance;
        Vector2 direction = target.RootPosition - RootPosition;
        bool hasTarget = Target != null;
        bool hasLOS = HasLineOfSight(target);
        bool isPlayerBelowEnemy = target.RootPosition.y + 1 < RootPosition.y;
        bool isAtBack = direction.x * Direction <= 0;

        //敌人发现目标的条件：如果已经锁定目标，只要不丢视线就能继续索敌；否则需要有视线，并且玩家不在敌人背后（防止敌人发现玩家后转身就丢失目标）
        //当玩家位置比敌人更低时，一定不在同一平台上，所以自动脱锁
        bool targetLocked = (hasTarget && hasLOS && !isPlayerBelowEnemy) || (hasLOS && !isPlayerBelowEnemy && !isAtBack);


        if (targetLocked)
        {
            Target = target;
            TargetPosition = Target.RootPosition;
        }
        else
        {
            Target = null;
        }
    }

    void SelectAction()
    {
        if (actions.Length > 0)
        {
            int randomIndex = Random.Range(0, actions.Length);
            actionCurrent = actions[randomIndex];
            actionCurrent.StartAction(ActionEnd);
        }
    }
    void ActionEnd()
    {
        actionCurrent = null;
    }

    public void Turn() => ((EnemyMove)moveComponent).Turn();
}
