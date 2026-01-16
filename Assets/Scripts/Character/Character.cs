using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


[RequireComponent(typeof(Rigidbody2D))]
public class Character : MonoBehaviour
{
    //是否加入单位统计
    public bool countByManager = true;

    //组件
    [PropertyOrder(5)] public CharacterMove moveComponent;

    //————————阵营————————
    [ShowInInspector] Faction faction;
    public Faction Faction { get => faction; }

    //————————阵营————————



    //————————朝向————————
    [ShowInInspector, ReadOnly]
    int direction = 1;
    public int Direction
    {
        get
        {
            return direction;
        }
        private set
        {
            direction = value;
        }
    }

    //————————朝向————————



    //————————时间相关————————
    float localTimeScale = 1;
    [ShowInInspector,ReadOnly]
    float timeScale = 1;
    public float TimeScale
    {
        get
        {
            timeScale = localTimeScale * TimeManager.GlobalTimeScale;
            return localTimeScale * TimeManager.GlobalTimeScale;
        }
        set
        {
            localTimeScale = value / TimeManager.GlobalTimeScale;
        }
    }
    //帧间隔
    public float FixedFrameInterval => Time.fixedDeltaTime * TimeScale;

    public float FrameInterval => Time.deltaTime * TimeScale;

    //————————时间相关————————


    //————————物理系统————————
    [PropertyOrder(5)] public Collider2D moveCollider;
    [PropertyOrder(5)] public Collider2D hitCollider;
    Rigidbody2D rigidbody2D;
    public Rigidbody2D Rigidbody
    {
        get
        {
            if (rigidbody2D == null)
                rigidbody2D = GetComponent<Rigidbody2D>();
            return rigidbody2D;
        }
    }

    //速度需要根据局部时间流速做单独调整（缓速下的物体的实际速度与Rigidbody速度不同）
    public Vector2 Velocity
    {
        get
        {
            return moveComponent.Velocity / TimeScale;
        }
        set
        {
            moveComponent.Velocity = value * TimeScale;
        }
    }


    //————————物理系统————————

    //————————动画系统————————
    Animator animator;
    public Animator Animator
    {
        get
        {
            if (animator == null)
                animator = GetComponentInChildren<Animator>();
            return animator;
        }
    }
    //————————动画系统————————


    //————————动作屏蔽————————
    //动作屏蔽
    [SerializeField, PropertyOrder(-2)] LinkedList<ActionIgnore> actionIgnores;

#if UNITY_EDITOR
    [ShowInInspector, LabelText("Action Ignores"), MultiLineProperty, PropertyOrder(-2)]

    string ActionIgnoreInspector
    {
        get
        {
            if (actionIgnores != null)
            {
                //ActionIgnore[] inspector = new ActionIgnore[actionIgnores.Count];
                string inspector = "";
                int i = 0;
                int index;
                List<string> tags = new List<string>();
                List<float> timers = new List<float>();
                foreach (ActionIgnore ignore in actionIgnores)
                {
                    if (tags.Contains(ignore.MaskToString))
                    {
                        index = tags.IndexOf(ignore.MaskToString);
                        timers[index] = Mathf.Max(timers[index], ignore.timer);
                    }
                    else
                    {
                        tags.Add(ignore.MaskToString);
                        timers.Add(ignore.timer);
                    }
                }

                for (int j = 0; j < tags.Count; j++)
                {
                    inspector += tags[j] + "(" + timers[j] + ")" + ", \n";
                    ++i;
                }
                return inspector;
            }
            else return "";
        }
    }

#endif


    public void RefreshActionIgnore()
    {
        for (var node = actionIgnores.First; node != null;)
        {
            //动作忽略标签自减
            node.Value.timer -= FixedFrameInterval;

            //移除到期的忽略标签
            if (node.Value.timer <= 0)
            {
                var next = node.Next;
                actionIgnores.Remove(node);
                if (node.Next == null) break;
                node = next;
            }
            node = node.Next;
        }
    }


    public bool IsIgnore(ActionIgnoreTag tag)
    {
        foreach (ActionIgnore ignore in actionIgnores)
        {
            if (ignore.mask.ContainTag(tag))
                return true;
        }
        return false;
    }
    public void AddIgnore(float time, params ActionIgnoreTag[] actionIgnoreTags)
    {
        ActionIgnoreMask mask = ActionIgnoreMask.GetMask(actionIgnoreTags);
        bool hasIgnore = false;
        foreach (ActionIgnore ignore in actionIgnores)
        {
            if (ignore.mask == mask)
            {
                //时间选一个更长的
                if (ignore.timer <= time)
                    ignore.timer = time;
                //不再生成新的Ignore
                hasIgnore = true;
            }
        }
        if (!hasIgnore)
            actionIgnores.AddFirst(new ActionIgnore(mask, time));
    }
    //————————动作屏蔽————————


    //————————位置相关————————

    public CharacterState characterState;
    [PropertyOrder(5)] public CharacterStateSensor characterStateSensor;

    public bool IsOnGround => characterState.isOnGround;
    public bool IsFacingWall => characterState.isFacingWall;

    [ShowInInspector]
    public Vector2Int GridPosition
    {
        get => (transform.position + new Vector3(0, 0.5f, 0)).GetMapGridPos();
    }

    [ShowInInspector]
    public Vector2 RootPosition
    {
        get => (transform.position + new Vector3(0, 0.1f, 0));
    }
    [ShowInInspector]
    public virtual Vector2 ChestPosition => (transform.position + new Vector3(0, 1.8f, 0));

    Timer turnDelay;

    //————————位置相关————————


    protected virtual void Awake()
    {
        //设置方向
        if (transform.lossyScale.x > 0)
            SetDirection(1);
        else
            SetDirection(-1);

        //设置时间流速
        localTimeScale = 1;

        //添加动作屏蔽
        actionIgnores = new LinkedList<ActionIgnore>();

        //角色状态
        characterState = new CharacterState();
    }

    protected virtual void Start()
    {
        if (countByManager)
            UnitManager.AddUnit(this);

        //设置计时器
        turnDelay = new Timer(this);
    }

    protected virtual void Update()
    {

    }

    protected virtual void FixedUpdate()
    {
        RefreshActionIgnore();
        //刷新角色状态
        characterStateSensor.RefreshFixedUpdate();
    }


    //移动分区
    public void ReverseDirection(float delay = 0) => SetDirection(-Direction, delay);
    public virtual void SetDirection(float direction, float delay = 0)
    {
        Vector3 size = transform.localScale;
        size.x = Mathf.Abs(size.x);
        if (direction > 0) Direction = 1;
        else if (direction < 0) Direction = -1;
        size.x *= Direction;
        transform.localScale = size;
        if (delay > 0)
        {
            turnDelay.Set(delay);
        }
    }

    public virtual void ForceMove(Displacement displacement)
    {
        moveComponent.SetForcedMove(displacement);
    }

    public bool HasLineOfSight(Character target)
    {
        Vector2 direction;
        bool hasLOS = false;
        direction = target.ChestPosition - ChestPosition;
        hasLOS |= !Physics2D.Raycast(ChestPosition, direction.normalized, direction.magnitude,
            UnityEngine.LayerMask.GetMask("Ground", "Wall"));
        direction = target.ChestPosition + new Vector2(1, 0) - ChestPosition;
        hasLOS |= !Physics2D.Raycast(ChestPosition, direction.normalized, direction.magnitude,
            UnityEngine.LayerMask.GetMask("Ground", "Wall"));
        direction = target.ChestPosition + new Vector2(-1, 0) - ChestPosition;
        hasLOS |= !Physics2D.Raycast(ChestPosition, direction.normalized, direction.magnitude,
            UnityEngine.LayerMask.GetMask("Ground", "Wall"));

        return hasLOS;
    }
    public bool HasLineOfSight(Vector2 target)
    {
        Vector2 direction;
        bool hasLOS = false;
        direction = target - ChestPosition;
        hasLOS |= !Physics2D.Raycast(ChestPosition, direction.normalized, direction.magnitude,
            UnityEngine.LayerMask.GetMask("Ground", "Wall"));

        return hasLOS;
    }

    public float GetDistance(Character character)
    {
        return GetDistance(character.ChestPosition);
    }
    public float GetDistance(Vector2 target)
    {
        return (target - ChestPosition).magnitude;
    }
    public bool IsBackTo(Character character)
    {
        return IsBackTo(character.ChestPosition);
    }

    public bool IsBackTo(Vector2 position)
    {
        float x = position.x - ChestPosition.x;
        return x * Direction < 0;
    }
}