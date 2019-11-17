using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_Poison : Effect
{
    public float PoisonAmount;

    private void Start()
    {
        turns = 5;
        onSelf = false;
    }

    public override void ActivateEffect()
    {
        player.TakeDamage(PoisonAmount);
    }
}
