using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public int turns;
    public bool onSelf;
    public Player player;

    public virtual void ApplyEffect(Player p)
    {
        turns = 1;
        player = p;
    }

    public virtual void ActivateEffect()
    {

    }

    public int GetTurns()
    {
        return turns;
    }

    public bool ApplyOnSelf()
    {
        return onSelf;
    }

    public void SetPlayer(Player p)
    {
        player = p;
    }
}
