using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class E_Kamikazee : Effect
{

    private void Start()
    {
        onSelf = false;
    }

    public override void ActivateEffect()
    {
        player.TakeDamage(player.GetHealth());
    }
}
