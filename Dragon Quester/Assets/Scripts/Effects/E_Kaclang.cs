using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_Kaclang : Effect
{

    private void Start()
    {
        onSelf = true;
    }

    public override void ActivateEffect()
    {
        player.MakeInvincible();
    }
}
