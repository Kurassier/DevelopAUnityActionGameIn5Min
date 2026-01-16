using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : PlayerComponent
{
    int horizontalMoveLastFrame = 0;

    //每次Update调用，获取输入数据
    public override void RefreshUpdate()
    {

        //————————水平移动输入————————
        //AD键同时按下时，以最后按下的键为准，保证玩家移动不停顿
        if (Input.GetKeyDown(KeyCode.A))
        {
            horizontalMoveLastFrame = -1;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            if (horizontalMoveLastFrame == -1)
            {
                if(!Input.GetKey(KeyCode.D))
                    horizontalMoveLastFrame = 0;
                else
                    horizontalMoveLastFrame = 1;
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            horizontalMoveLastFrame = 1;
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            if (horizontalMoveLastFrame == 1)
            {
                if (!Input.GetKey(KeyCode.A))
                    horizontalMoveLastFrame = 0;
                else
                    horizontalMoveLastFrame = -1;
            }
        }
        input.horizontalMove = horizontalMoveLastFrame;
        //————————水平移动输入————————

        input.horizontalMove = (int)Input.GetAxisRaw("Horizontal");

        //动作输入
        input.jump = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W);
    }
}

[System.Serializable]
public class InputData
{
    public int horizontalMove;
    public bool isMoving;
    public bool jump;
    public bool attack;
}