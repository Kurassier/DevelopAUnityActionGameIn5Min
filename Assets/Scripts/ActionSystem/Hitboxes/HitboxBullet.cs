using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HitboxBullet : Hitbox
{

    #region 常量数据
    #endregion

    [SerializeField]
    GameObject blockedSpark;
    [SerializeField]
    GameObject hitSpark;
    [SerializeField]
    GameObject parryEffect;

    [SerializeField]
    GameObject launchSmoke;

    [SerializeField]
    bool hasDebris = true;

    protected new Rigidbody2D rigidbody;

    public float originDamage = 0;
    public int remainPenetration = 0;
    public int originPenetraion = 0;

    Vector3 hitPoint = new Vector3(float.NaN, float.NaN, float.NaN);
    iDamagable hitTarget = null;

    protected override void Awake()
    {
        base.Awake();
        rigidbody = GetComponent<Rigidbody2D>();

        if (launchSmoke != null)
        {
            GameObject smoke = Instantiate(launchSmoke, transform.position, transform.rotation, null);
            smoke.transform.localScale = new Vector3(1, 1, 1);
        }

        originDamage = remainDamage;
        originPenetraion = remainPenetration;
    }

    public override Vector2 ImpactDirection => rigidbody.velocity.normalized;

    bool isFirstFrame = true;
    protected override List<iDamagable> CollideCheck()
    {
        //第一帧不判定伤害，不知道当时为啥写，估计防止自己打自己，不过似乎已经通过其他方法解决了该问题
        //反而会导致子弹穿墙，所以暂时禁用，以后看情况
        //if (isFirstFrame)
        //{
        //    isFirstFrame = false;
        //    return new List<Character>();
        //}
        //检测是否已经被摧毁
        if (gameObject.layer == UnityEngine.LayerMask.NameToLayer("Debris"))
        {
            return new List<iDamagable>();
        }

        List<Collider2D> result = new List<Collider2D>();
        List<iDamagable> hit = new List<iDamagable>();
        ContactFilter2D filter = new ContactFilter2D();
        //filter.SetLayerMask(LayerMask.GetMask("Character"));

        //子弹类的改为使用射线检测
        RaycastHit2D[] raycastHits = Physics2D.RaycastAll(transform.position, rigidbody.velocity,
            rigidbody.velocity.magnitude * Time.fixedDeltaTime * 1.0f);

        List<RaycastHit2D> allHits = raycastHits.ToList<RaycastHit2D>();

        //依照距离排序
        allHits.Sort((x, y) => (x.point - (Vector2)transform.position).magnitude.CompareTo((y.point - (Vector2)transform.position).magnitude));
        foreach (RaycastHit2D raycastHit in allHits)
        {
            Collider2D collider = raycastHit.collider;
            //已经结算过碰撞的碰撞体不再重新结算
            if (!touchedColliders.Contains(collider))
            {
                //判断是否属于某个角色
                bool isInvincibleCharacter = false;
                iDamagable target = collider.GetComponentInParent<iDamagable>();
                if (target != null)
                {
                    //玩家只有Player层的碰撞体有伤害判定
                    if (target.Faction == Faction.player && collider.gameObject.layer != UnityEngine.LayerMask.NameToLayer("Player"))
                    {
                        continue;
                    }

                    if (targetFaction.Contains(target.Faction))
                    {
                        if (target.HasLineOfSight(transform.position))
                            hit.Add(target);

                    }
                }
                else
                {
                    isInvincibleCharacter = true;
                }

                //判断是否是地图边界，或者无敌的角色
                if (collider.gameObject.layer == UnityEngine.LayerMask.NameToLayer("Ground"))
                {
                    //与地图边界碰撞，碰撞点要修正到接触点
                    hitPoint = raycastHit.point;
                    hitPoint.z = transform.position.z;
                    hitPoint += new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0);
                    //如果是地图边界，直接执行Stucked逻辑
                    Stucked(HitResultType.Stucked);
                    break;
                }

                touchedColliders.Add(collider);
            }

        }
        return hit;
    }

    protected virtual float bounceFactor => 0.5f;
    protected override void HitResultCheck(List<iDamagable> hitTargets)
    {

        //依次结算命中
        HitResultType type = HitResultType.Stucked;
        //射线检测的距离排序按照HitPoint的距离进行，在导入之前就已经排好了
        foreach (iDamagable target in hitTargets)
        {
            HitResult result = Hit(target, remainDamage);
            remainDamage -= result;
            //Block的命中，无论剩余伤害与穿透数，都立即停止，玩家格挡子弹还会有额外的弹反判定
            if (result.hitResultType == HitResultType.Blocked)
            {
                type = HitResultType.Blocked;
                hitTarget = target;
                Stucked(type);
                break;
            }
            //Miss的命中，继续飞行
            else if (result.hitResultType == HitResultType.Miss) { }
            //远程攻击命中，是否停止不依照剩余伤害，而是单独检验角色穿透数
            else
            {
                remainPenetration--;
                if (remainPenetration <= 0)
                {
                    type = HitResultType.Stucked;
                    hitTarget = target;
                    Stucked(type);
                    break;
                }
            }
            //else if (result.hitResultType == HitResultType.Stucked)
            //{
            //    type = HitResultType.Stucked;
            //    Stucked(type);
            //    break;
            //}
            //else if(result.hitResultType == HitResultType.Hit)
            //{
            //    type = HitResultType.Stucked;
            //    Stucked(type);
            //    break;
            //}
        }

    }

    protected override HitResult Hit(iDamagable target, float remainDamage)
    {
        HitResult result = base.Hit(target, remainDamage);

        //命中特效只播一次
        //子弹被格挡不播放特效
        if (!isHit && result.hitResultType != HitResultType.Miss && result.hitResultType != HitResultType.Blocked)
        {
            PlayHitEffect();
            isHit = true;
        }
        return result;
    }


    public override void Stucked(HitResultType type)
    {
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        Vector2 velocity = rigidbody.velocity;

        Quaternion thisRotationWithRandom = Quaternion.Euler(0, 0, Random.Range(-20f, 20f)) * transform.rotation;

        if (type == HitResultType.Stucked)
        {
            //命中的子弹穿透不反弹
            velocity = velocity / 5;
            velocity.y = velocity.y / 3;
            rigidbody.AddTorque(Random.Range(-50, 50));

            velocity += new Vector2(Random.Range(-1, 1), Random.Range(0, 3)) * velocity.magnitude / 3;

            if (hitSpark != null)
            {
                if (hitPoint.IsValid())
                    GameObject.Instantiate(hitSpark, hitPoint, thisRotationWithRandom, null);
                else
                    GameObject.Instantiate(hitSpark, transform.position, thisRotationWithRandom, null);
            }
        }
        else if (type == HitResultType.Blocked)
        {


            //格挡的子弹反弹不穿透
            velocity = -velocity / 3;
            rigidbody.AddTorque(Random.Range(-150, 150));

            velocity += new Vector2(Random.Range(-2, 2), Random.Range(0, 3)) * velocity.magnitude / 2;

            if (blockedSpark != null)
            {
                if (hitPoint.IsValid())
                    GameObject.Instantiate(blockedSpark, hitPoint, thisRotationWithRandom, null);
                else
                    GameObject.Instantiate(blockedSpark, transform.position, thisRotationWithRandom, null);
            }

        }
        else if (type == HitResultType.Counter)
        {
            //速度反弹
            rigidbody.velocity *= -bounceFactor;
            //锁定速度为80
            rigidbody.velocity = rigidbody.velocity.normalized * 80;
            //关闭阻力
            rigidbody.drag = 0;

            //重置伤害值（无论初始多少，都调整为5）
            remainDamage = 5;
            remainPenetration = originPenetraion;
            //重置目标阵营
            targetFaction = Faction.enemy;
            //重置经过的碰撞体
            touchedColliders.Clear();
            //重置伤害来源
            //origin = Player.main;


            //直接返回，不执行后续的销毁操作
            return;
        }

        rigidbody.velocity = velocity;
        rigidbody.gravityScale = 8;

        Collider2D collider = GetComponent<Collider2D>();
        collider.isTrigger = false;

        TrailRenderer trail = GetComponent<TrailRenderer>();
        Destroy(trail);

        gameObject.layer = UnityEngine.LayerMask.NameToLayer("Debris");

        Destroy(this);

        if (!hasDebris && type == HitResultType.Stucked)
        {
            Destroy(gameObject);
        }
    }


    public override void Destroy()
    {

        base.Destroy();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (rigidbody != null)
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)(rigidbody.velocity * Time.fixedDeltaTime * 2));
    }
}
