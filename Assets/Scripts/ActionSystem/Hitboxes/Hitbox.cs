using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    #region 常量数据
    //————通用————
    //是否有友军伤害
    public bool hasFriendlyDamage = false;
    //伤害类型
    public DamageType damageType = DamageType.Melee;
    //最大持续时间
    public float maxExistTime = 0.1f;

    //————近战————
    //是否跟随发起者
    public bool isFollowOwner = false;
    //跟随时间
    public float followTime = -1;

    //————远程————
    //是否是飞行物
    public bool isProjectile = false;
    //初速度
    public float initialSpeed = 0;
    //阻力系数
    public float dragFactor = 0;
    //最大速度
    public float maxSpeed = 0;
    //最小速度
    public float minSpeed = 0;
    //追踪最大角速度
    public float tracingSpeed = 0;



    //————伤害与冲击力————
    //伤害值
    public float damage = 1;
    //击退冲量
    public float impulse = 0;
    //冲击力类别
    public ImpactType impactType = ImpactType.None;
    //冲击力方式（平行、离心）
    public ImpulseType impulseType = ImpulseType.Parallel;


    //————打击反馈————
    //相机抖动幅度
    public float cameraShakeMagnitude = 0;
    //相机抖动次数
    public int cameraShakeRepeat = 0;
    //相机抖动时长
    public float cameraShakeTime = 0;
    //帧冻结时间倍率
    public float frameFreezeFactor = 1;
    //帧冻结时长
    public float frameFreezeLength = 0;
    //帧冻结回复时长
    public float frameFreezeRecoverLength = 0;
    #endregion


    #region 变量数据
    //可受击的阵营
    public Faction targetFaction;
    //剩余伤害
    [SerializeField] protected float remainDamage = 10;
    //自毁计时器
    protected float destroyTimer;
    //伤害来源
    public Character origin = null;
    #endregion

    //冲击方向
    public virtual Vector2 ImpactDirection => new Vector2(0, 1);




    //攻击碰撞箱的映射关系
    static GameobjectReflectionData hitboxReflection;
    static GameobjectReflectionData HitboxReflection
    {
        get
        {
            if (hitboxReflection == null) hitboxReflection = GameobjectReflectionData.GetReflectionData("Hitbox");
            return hitboxReflection;
        }
    }

    //已经碰撞过的碰撞体，不再反复触发碰撞事件
    public List<Collider2D> touchedColliders;

    //击中效果是否已经播放
    protected bool isHit = false;

    protected virtual void Awake()
    {
        touchedColliders = new List<Collider2D>();

        hitItems = new List<iDamagable>();
    }



    protected virtual void FixedUpdate()
    {
        //销毁计时器
        destroyTimer -= Time.fixedDeltaTime;
        if (destroyTimer < 0)
            Destroy();

        //如果当前已经达到上限，不做任何行为
        if (remainDamage <= 0) return;


        //碰撞检测
        List<iDamagable> hitTargets = CollideCheck();

        //对命中的角色进行排序
        //hitTargets.Sort((x, y) => (x.ChestPosition - (Vector2)transform.position).magnitude.CompareTo
        //    ((y.ChestPosition - (Vector2)transform.position).magnitude));

        //依次结算命中
        HitResultCheck(hitTargets);

    }
    protected virtual List<iDamagable> CollideCheck()
    {
        List<iDamagable> hit = new List<iDamagable>();
        return hit;
    }

    protected virtual void HitResultCheck(List<iDamagable> hitTargets)
    {

    }


    public List<iDamagable> hitItems;
    protected virtual HitResult Hit(iDamagable target, float remainDamage)
    {
        //每个角色只能判定一次，因为物理系统有些没找到的BUG，角色偶尔会判定多次
        if (hitItems.Contains(target))
            return new HitResult(0, HitResultType.Miss);
        hitItems.Add(target);

        //生成伤害参数
        Vector2 impulseVector = new Vector2();
        if (impulseType == ImpulseType.Parallel)
        {
            impulseVector = new Vector2(Mathf.Cos(transform.eulerAngles.z.ToRadius()),
                Mathf.Sin(transform.eulerAngles.z.ToRadius()));

            if (transform.lossyScale.x < 0)
                impulseVector *= -1;
        }
        else if (impulseType == ImpulseType.Centrifugal)
            impulseVector = transform.position;

        Damage damage = new Damage(remainDamage, origin, damageType, impulse, impulseVector, impulseType, impactType);


        //触发角色受击
        HitResult result = target.Hit(damage);

        return result;
    }
    protected virtual void PlayHitEffect()
    {
        //帧冻结
        TimeManager.FrameFreeze(frameFreezeLength, frameFreezeRecoverLength, frameFreezeFactor);
        //镜头晃动
        CameraShake.Shake(cameraShakeMagnitude, cameraShakeRepeat, cameraShakeTime, ImpactDirection);
    }


    public virtual void Stucked(HitResultType type)
    {

    }

    public virtual void Destroy()
    {
        GameObject.Destroy(gameObject);
    }


    /// <summary>
    /// 生成新的攻击碰撞箱
    /// </summary>
    /// <param name="hitboxPrefab">预制件</param>
    /// <param name="template">数据模板</param>
    /// <param name="origin">伤害来源</param>
    /// <param name="launcher">发射者，如果碰撞箱跟随发射者移动时启用</param>
    /// <param name="damage">伤害值</param>
    /// <param name="position">位置</param>
    /// <param name="degree">角度</param>
    /// <returns>生成的攻击碰撞箱</returns>
    public static Hitbox GenerateHitbox(GameObject hitboxPrefab ,Character origin,
        Transform launcher, float damage, Vector3 position, float degree = 0)
    {
        // 实例化碰撞箱
        Hitbox hitbox = GameObject.Instantiate(hitboxPrefab, position, Quaternion.Euler(0, 0, degree), null).GetComponent<Hitbox>();

        // 设置伤害来源
        hitbox.origin = origin;
        // 设置剩余伤害
        hitbox.remainDamage = hitbox.damage;
        // 设置起效的阵营（是否开启友军伤害）
        if (hitbox.hasFriendlyDamage)
            hitbox.targetFaction = Faction.all;
        else
            hitbox.targetFaction = origin.Faction.GetHostileFaction();
        // 碰撞箱是否跟随发射者
        if (hitbox.isFollowOwner) hitbox.transform.parent = launcher;
        // 计时器，定时销毁碰撞箱
        hitbox.destroyTimer = hitbox.maxExistTime;
        return hitbox;
    }

}

public class HitResult
{
    public float damageAbsorb = 1;
    public HitResultType hitResultType = HitResultType.Hit;

    public HitResult(float damageAbsorb, HitResultType hitResultType = HitResultType.Hit)
    {
        this.damageAbsorb = damageAbsorb;
        this.hitResultType = hitResultType;
        if (damageAbsorb == 0)
            this.hitResultType = HitResultType.Miss;
    }

    public static implicit operator HitResult(float damage)
    {
        return new HitResult(damage);
    }

    public static implicit operator float(HitResult result)
    {
        return result.damageAbsorb;
    }

}

public enum HitResultType { Miss, Hit, Stucked, Blocked, Counter }