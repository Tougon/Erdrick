using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] FightController FC;
    [SerializeField] int PlayerID;
    [SerializeField] GameObject UIelements;
    [SerializeField] Text healthText, mpText;

    [HideInInspector] public enum Command { Attack, Block, Spell };

    float Health, MP;

    float atkDamage;

    Command Action;
    Spell currentSpell;
    List<Spell> spellList;

    bool canDoThings;

    void StartBattle()
    {
        atkDamage = 10.0f;
    }

    public void StartTurn()
    {
        ShowUI();
        canDoThings = true;
    }

    void ShowUI()
    {
        // show the ui here and the player can do anything
        UIelements.SetActive(true);
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
        UIelements.SetActive(true);
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

    public Spell GetSpell()
    {
        return currentSpell;
    }

    public float GetDamage()
    {
        return atkDamage;
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
        healthText.text = Health.ToString();
        if(Health <= 0.0f)
        {
            PlayerDied();
        }
    }

    public void MagicCastSuccess()
    {
        mpText.text = (MP - currentSpell.GetCost()).ToString();
    }

    void PlayerDied()
    {
        FC.SendMessage("PlayerDied", 0);
    }

    void EndBattle()
    {
        
    }
}
