using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_Kaheal : Effect
{
    public float HealAmount;

    private void Start()
    {
        turns = 4;
        onSelf = true;
    }

    public override void ActivateEffect()
    {
        player.Heal(HealAmount);
    }
}
