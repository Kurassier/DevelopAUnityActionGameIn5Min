using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerCamera : PlayerComponent
{
    public Vector3 localOffset = new Vector3(0, 5, -100);
    public float forwardRange = 3;
    public float forwardSpeed = 3;
    public float forwardCameraTime = 2;

    public float deadzoneX = 0.2f;
    public float deadzoneY = 0.2f;
    public float damping = 0.1f;

    public CinemachineVirtualCamera virtualCamera;

    Vector3 positionLastFrame;
    Transform followTransform;
    [SerializeField] float forwardRangeCurrent = 0;
    float forwardCameraTimer = 0;
    Direction lastFrameDirection = 0;

    public override void Init()
    {
        base.Init();
        followTransform = transform.parent;
        transform.parent = null;
        transform.position = followTransform.position + localOffset;
    }

    public override void RefreshUpdate()
    {
        //相机参数
        float widthHeightRatio = (float)Screen.width / (float)Screen.height;
        float height = virtualCamera.m_Lens.OrthographicSize;
        float width = height * widthHeightRatio;

        //————————死区————————
        float followSpeed = damping != 0 ? 1 / damping : float.MaxValue;


        Vector3 targetPosition = followTransform.position + localOffset;
        Vector3 position = positionLastFrame;

        //前置相机
        forwardCameraTimer -= Time.deltaTime;
        if (Player.Instance.Direction != lastFrameDirection)
        {
            forwardCameraTimer = forwardCameraTime;
            lastFrameDirection = Player.Instance.Direction;
        }
        if (forwardCameraTimer > 0)
            forwardRangeCurrent = Mathf.MoveTowards(forwardRangeCurrent, 0, forwardSpeed * Time.deltaTime);
        else
            forwardRangeCurrent = Mathf.MoveTowards(forwardRangeCurrent, forwardRange * lastFrameDirection, forwardSpeed * Time.deltaTime);
        targetPosition += new Vector3(forwardRangeCurrent, 0, 0);

        if (targetPosition.x - position.x > deadzoneX * width)
            position.x = Mathf.Lerp(position.x, targetPosition.x - deadzoneX * width, followSpeed * Time.deltaTime);
        if (targetPosition.x - position.x < -deadzoneX * width)
            position.x = Mathf.Lerp(position.x, targetPosition.x + deadzoneX * width, followSpeed * Time.deltaTime);

        if (targetPosition.y - position.y > deadzoneY * height)
            position.y = Mathf.Lerp(position.y, targetPosition.y - deadzoneY * height, followSpeed * Time.deltaTime);
        if (targetPosition.y - position.y < -deadzoneY * height)
            position.y = Mathf.Lerp(position.y, targetPosition.y + deadzoneY * height, followSpeed * Time.deltaTime);

        position.z = localOffset.z;
        transform.position = position;

        //记录当前帧相机的世界坐标
        positionLastFrame = transform.position;
        //————————死区————————


        //最后添加相机抖动，以免影响死区
        transform.localPosition += (Vector3)CameraShaker.ShakeOffset;
    }

    void OnDrawGizmosSelected()
    {
        float widthHeightRatio = (float)Screen.width / (float)Screen.height;
        Vector3 center = transform.position - localOffset + new Vector3(0, 1, 0);
        float height = virtualCamera.m_Lens.OrthographicSize;
        float width = height * widthHeightRatio;
        Vector3 deadzone = Vector3.zero;

        //死区边界（延长线）
        Gizmos.color = Color.grey;
        deadzone = new Vector3(width * 1, height * deadzoneY, 0);
        Gizmos.DrawWireCube(center, deadzone * 2);
        center = transform.position;
        deadzone = new Vector3(width * deadzoneX, height * 1, 0);
        Gizmos.DrawWireCube(center, deadzone * 2);

        //死区边界
        Gizmos.color = Color.red;
        center = transform.position - localOffset + new Vector3(0, 1, 0);
        deadzone = new Vector3(width * deadzoneX, height * deadzoneY, 0);
        Gizmos.DrawWireCube(center, deadzone * 2);
    }
}
