using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class E_Kamikazee : Effect
{

    private void Start()
    {
        turns = 1;
        onSelf = false;
    }

    public override void ActivateEffect()
    {
        player.TakeDamage(500);
    }
}
