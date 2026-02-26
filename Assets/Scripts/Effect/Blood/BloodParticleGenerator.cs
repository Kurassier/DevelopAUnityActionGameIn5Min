using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodParticleGenerator : Singleton<BloodParticleGenerator>
{

    public GameObject bloodOnBackground;
    public GameObject bloodOnWall;
    public GameObject bloodParticle;

    public Sprite[] bloodsOnBackground;
    public Sprite[] bloodsOnWall;

    protected override void Awake()
    {
        base.Awake();
        transform.parent = null;
    }


    public void GenerateBloodOnBackground(Vector3 position)
    {
        //通过小幅度随机一些位置和旋转参数，让效果更多样
        //随机位置
        position += new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.5f, 0.5f), 0) * 3;
        //随机角度
        float angle = Random.Range(-20f, 20f);
        Vector2 size = new Vector2(Random.Range(0.8f, 1.2f), Random.Range(0.8f, 1.2f));
        //随机选一个方向，-1或1
        int direction = Random.Range(-1, 1) * 2 - 1;

        //生成物体
        GameObject blood = Instantiate(bloodOnBackground, position + new Vector3(0, 0, -0.6f), Quaternion.Euler(0, 0, angle), transform);
        //设置方向
        if (direction == -1) size.x *= -1;
        blood.transform.localScale = new Vector3(size.x, size.y, 1);

        //设置血液图片（从预设数组里随机挑一张）
        Sprite sprite = null;
        sprite = bloodsOnBackground[Random.Range(0, bloodsOnBackground.Length)];
        blood.GetComponent<SpriteRenderer>().sprite = sprite;
    }
    public void GenerateBloodOnWall(Vector3 position, Vector2 normal)
    {
        Vector2 size = new Vector2(1, 1);
        //根据法线方向，计算旋转角度
        float angle = Mathf.Atan2(normal.y, normal.x) * 180 / Mathf.PI - 90;

        //生成物体（需要一小段Z轴位移，保证在地形前方）
        GameObject blood = Instantiate(bloodOnWall, position + new Vector3(0, 0, -0.6f), Quaternion.Euler(0, 0, angle), transform);

        //设置血液图片（从预设数组里随机挑一张）
        Sprite sprite = null;
        sprite = bloodsOnWall[Random.Range(0, bloodsOnWall.Length)];
        blood.GetComponent<SpriteRenderer>().sprite = sprite;
    }

    public void GenerateBloodParticle(Vector3 position, Vector2 velocity)
    {
        Vector2 size = new Vector2(1, 1);
        float angle = Mathf.Atan2(velocity.y, velocity.x) * 180 / Mathf.PI;

        //生成物体
        GameObject blood = Instantiate(bloodParticle, position, Quaternion.Euler(0, 0, angle), transform);
        BloodParticle particle = blood.GetComponent<BloodParticle>();
        particle.velocity = velocity;
    }
}
