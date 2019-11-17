using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Effect : MonoBehaviour
{
    public int turns;
    public bool onSelf;
    public Player player;
    public Sprite eff1, eff2;

    public virtual void ApplyEffect(Player p)
    {
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
