using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] FightController FC;
    [SerializeField] int PlayerID;

    [HideInInspector] public enum Command { Attack, Block, Spell };

    float Health, MP;

    Command Action;
    Spell currentSpell;
    List<Spell> spellList;

    bool canDoThings;

    void StartBattle()
    {

    }

    void StartTurn()
    {
        ShowUI();
    }

    void ShowUI()
    {
        // show the ui here and the player can do anything
        canDoThings = true;
    }

    void SelectAction(Command selectedCommand, int selectedSpell)
    {
        Action = selectedCommand;
        if (Action == Command.Spell)
        {
            currentSpell = spellList[selectedSpell];
            if(MP > currentSpell.GetCost())
            {
                HideUI();
            }
            else
            {
                // player didn't have enough mp, don't do anything
            }
        }
        else
        {
            HideUI();
        }
    }

    void HideUI()
    {
        // hide the ui here and the player can't do anything
        canDoThings = false;
        SendCommand();
    }

    public void SendCommand()
    {
        FC.SendMessage("ReceiveCommand", PlayerID);
    }

    public Command GetAction()
    {
        return Action;
    }

    void TakeDamage(float damage)
    {
        Health -= damage;
        if(Health <= 0.0f)
        {
            PlayerDied();
        }
    }

    void PlayerDied()
    {
        FC.SendMessage("PlayerDied", 0);
    }

    void EndBattle()
    {
        
    }
}
