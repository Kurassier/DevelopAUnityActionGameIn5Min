using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerComponent : CharacterComponent
{
    public new Player Owner => (Player)base.Owner;
    public InputData input => Owner.input;


}
