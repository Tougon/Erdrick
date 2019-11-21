using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_Rest : Effect
{

    [SerializeField] float healAmount;

    private void Start()
    {
        onSelf = false;
    }

    public override void ActivateEffect()
    {
        player.Heal(healAmount);
        player.CantAct();
    }
}
