using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAction : CharacterComponent
{
    protected System.Action actionEndCallback = null;
    protected new Enemy Owner => (Enemy)base.Owner;

    public virtual void StartAction(System.Action onActionEnd)
    {
        actionEndCallback = onActionEnd;
    }

}
