using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class E_Snooze : Effect
{

    private void Start()
    {
        turns = 1;
        onSelf = false;
    }

    public override void ActivateEffect()
    {
        player.CantAct();
    }
}
