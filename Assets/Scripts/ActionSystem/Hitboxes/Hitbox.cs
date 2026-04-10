using Sirenix.OdinInspector;
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
    //帧冻结时长
    public float frameFreezeLength = 0;
    #endregion


    #region 变量数据
    //可受击的阵营
    [ReadOnly] public Faction targetFaction;
    //自毁计时器
    protected float destroyTimer;
    //伤害来源
    [ReadOnly] public Character origin = null;
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

    //在生成时直接调用，不需要在自身显式调用
    protected virtual void Init()
    {
        touchedColliders = new List<Collider2D>();
        alreadyHitItems = new List<iDamagable>();
    }

    protected virtual void FixedUpdate()
    {
        //销毁计时器
        destroyTimer -= origin == null ? Time.fixedDeltaTime : origin.FixedFrameInterval;
        if (destroyTimer < 0)
            Destroy();


        //碰撞检测
        List<iDamagable> hitTargets = CollideCheck();

        //结算命中
        HitResultCheck(hitTargets);

    }

    //碰撞检测，不同的Hitbox有不同的检测方法
    //检测后已经进行了排序，一般按照距离来判定结算顺序
    protected virtual List<iDamagable> CollideCheck()
    {
        List<iDamagable> hit = new List<iDamagable>();
        return hit;
    }

    protected virtual void HitResultCheck(List<iDamagable> hitTargets)
    {

    }

    //已经结算过碰撞的物体
    public List<iDamagable> alreadyHitItems;
    //结算伤害判定
    //包括实际施加伤害、生成特效等等
    protected virtual HitResult Hit(iDamagable target, float remainDamage)
    {
        //每个角色只能判定一次，因为角色一般有多个碰撞体，否则角色会判定多次
        if (alreadyHitItems.Contains(target))
            return new HitResult(0, HitResultType.Miss);
        alreadyHitItems.Add(target);

        //生成伤害参数——冲击力方向
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
        TimeManager.FrameFreeze(frameFreezeLength);
        //镜头晃动
        CameraShaker.Shake(cameraShakeMagnitude, cameraShakeRepeat, cameraShakeTime, ImpactDirection);
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
    /// <param name="parent">父物体，如果碰撞箱跟随发射者移动时启用</param>
    /// <param name="damage">伤害值</param>
    /// <param name="position">位置</param>
    /// <param name="degree">角度</param>
    /// <returns>生成的攻击碰撞箱</returns>
    public static Hitbox GenerateHitbox(GameObject hitboxPrefab, Character origin,
        Transform parent, float damage, Vector3 position, float degree = 0)
    {
        // 实例化碰撞箱
        Hitbox hitbox = GameObject.Instantiate(hitboxPrefab, position, Quaternion.Euler(0, 0, degree), null).GetComponent<Hitbox>();

        // 设置伤害来源
        hitbox.origin = origin;
        // 设置起效的阵营（是否开启友军伤害）
        if (hitbox.hasFriendlyDamage)
            hitbox.targetFaction = Faction.all;
        else
            hitbox.targetFaction = origin.Faction.GetHostileFaction();
        // 计时器，定时销毁碰撞箱
        hitbox.destroyTimer = hitbox.maxExistTime;
        // 设置伤害值
        hitbox.damage = damage;
        // 设置父物体
        hitbox.transform.parent = parent;
        hitbox.transform.localScale = Vector3.one;

        //初始化碰撞箱
        hitbox.Init();
        return hitbox;
    }

}

public class HitResult
{
    public float damageReceived = 1;
    public HitResultType hitResultType = HitResultType.Hit;

    public HitResult(float damageReceived, HitResultType hitResultType = HitResultType.Hit)
    {
        this.damageReceived = damageReceived;
        this.hitResultType = hitResultType;
    }

}

//打击结果，丢失、命中、卡刀、格挡（盾牌）、弹反
public enum HitResultType { Miss, Hit, Stucked, Blocked, Counter }