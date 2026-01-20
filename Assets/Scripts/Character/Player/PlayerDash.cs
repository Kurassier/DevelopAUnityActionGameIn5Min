using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : PlayerComponent
{
    const float DashCD = 0.3f;

    public DisplacementData displacementData;

    float dashPreinput = -1f;


    public override void RefreshUpdate()
    {
        dashPreinput -= Time.unscaledDeltaTime;
        if (input.dash)
        {
            dashPreinput = 0.2f;
        }
    }

    public override void RefreshFixedUpdate()
    {
        if (dashPreinput > 0f && !Owner.IsIgnore(ActionIgnoreTag.Dash))
        {
            dashPreinput = -1f;
            Dash();
        }
    }

    void Dash()
    {
        //动作屏蔽
        Owner.AddIgnore(DashCD, ActionIgnoreTag.Dash);
        Owner.AddIgnore(0.2f, ActionIgnoreTag.All);
        Owner.AddIgnore(0.2f, ActionIgnoreTag.Move);

        //播放动画
        if (Owner.characterState.isOnGround)
            Owner.Animator.Play("Slide", 0, 0);
        else
            Owner.Animator.Play("Dash", 0, 0);


        //强制移动
        Owner.ForceMove(displacementData);
    }
}