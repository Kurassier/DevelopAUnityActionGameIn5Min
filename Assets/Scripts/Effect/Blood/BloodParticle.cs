using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodParticle : MonoBehaviour
{
    public Sprite[] sprites;
    public Color startColor;
    public Color endColor;

    public Vector2 velocity;
    public float existTime = 0.75f;

    SpriteRenderer renderer;
    float t = 0;

    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
    }
    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;

        //根据t选择对应的图片
        int spriteIndex = Mathf.Clamp((int)(t * sprites.Length / existTime), 0, sprites.Length - 1);
        renderer.sprite = sprites[spriteIndex];
        //根据t选择对应的颜色
        Color color = Color.Lerp(startColor, endColor, Mathf.Clamp01(t / existTime));
        renderer.color = color;

        //模拟重力，保持粒子始终朝向运动方向
        velocity -= new Vector2(0, 10 * Time.deltaTime);
        float angle = Mathf.Atan2(velocity.y, velocity.x);
        transform.position += (Vector3)velocity * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, angle * 180 / Mathf.PI);

        //用射线检测是否碰撞到地形
        RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, velocity.normalized, velocity.magnitude * (1.5f * Time.deltaTime),
            LayerMask.GetMask("Ground", "Wall", "Platform"));
        if (raycastHit)
        {
            BloodParticleGenerator.Instance.GenerateBloodOnWall(raycastHit.point, raycastHit.normal);
            Destroy(gameObject);
        }

        //超时则自毁
        if (t >= existTime + 0.2f)
        {
            Destroy(gameObject);
        }
    }

}
