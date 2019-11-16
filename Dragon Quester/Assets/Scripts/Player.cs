using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] FightController FC;
    [SerializeField] int PlayerID;
    [SerializeField] GameObject CommandUI;
    [SerializeField] Text healthText, mpText, spellUp, spellDown, spellLeft, spellRight;

    [HideInInspector] public enum Command { Attack, Block, Spell };

    float Health, MP;

    float atkDamage;

    Command Action;
    Spell currentSpell;
    [SerializeField] List<Spell> spellList;

    bool canDoThings;

    private void Awake()
    {
        canDoThings = false;
    }

    private void Update()
    {
        #region Garbage Input Test Stuff
        if (canDoThings)
        {
            if ((Input.GetKeyDown(KeyCode.Q) && PlayerID == 0) || (Input.GetKeyDown(KeyCode.U) && PlayerID == 1))
            {
                Debug.Log("attak");
                SelectAction(Command.Attack, 0);
            }
            else if ((Input.GetKeyDown(KeyCode.E) && PlayerID == 0) || (Input.GetKeyDown(KeyCode.O) && PlayerID == 1))
            {
                Debug.Log("blok");
                SelectAction(Command.Block, 0);
            }
            else if ((Input.GetKeyDown(KeyCode.W) && PlayerID == 0) || (Input.GetKeyDown(KeyCode.I) && PlayerID == 1))
            {
                Debug.Log("spel");
                SelectAction(Command.Spell, 0);
            }
        }
        #endregion
    }

    public void StartBattle()
    {
        atkDamage = 10.0f;
        Health = 100.0f;
        MP = 100.0f;
        StartTurn();
    }

    public void StartTurn()
    {
        FillSpellList();
        ShowUI();
        canDoThings = true;
    }

    void FillSpellList()
    {
        for(int i = 0; i < spellList.Count; i++)
        {
            spellList[i] = FC.GetRandomSpell();
        }
        spellUp.text = spellList[0].GetName();
        spellDown.text = spellList[1].GetName();
        spellLeft.text = spellList[2].GetName();
        spellRight.text = spellList[3].GetName();
    }

    void ShowUI()
    {
        CommandUI.SetActive(true);
    }

    void SelectAction(Command selectedCommand, int selectedSpell)
    {
        Debug.Log("select action");
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
        Debug.Log("hide ui");
        CommandUI.SetActive(false);
        canDoThings = false;
        SendCommand();
    }

    public void SendCommand()
    {
        Debug.Log("send command");
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
        healthText.text = Health.ToString() + " / 100";
        if(Health <= 0.0f)
        {
            PlayerDied();
        }
    }

    public void MagicCastSuccess()
    {
        mpText.text = (MP - currentSpell.GetCost()).ToString() + " / 100";
    }

    void PlayerDied()
    {
        FC.SendMessage("PlayerDied", 0);
    }

    void EndBattle()
    {
        
    }
}
