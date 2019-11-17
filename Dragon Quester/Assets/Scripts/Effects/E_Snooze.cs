using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class E_Snooze : Effect
{

    private void Start()
    {
        onSelf = false;
    }

    public override void ActivateEffect()
    {
        player.CantAct();
    }
}
