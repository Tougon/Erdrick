using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_Heal : Effect
{

    public float HealAmount;

    private void Start()
    {
        turns = 1;
        onSelf = true;
    }

    public override void ActivateEffect()
    {
        player.Heal(HealAmount);
    }
}
