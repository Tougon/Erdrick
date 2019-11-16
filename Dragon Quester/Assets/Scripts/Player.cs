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
                Debug.Log("spel up");
                SelectAction(Command.Spell, 0);
            }
            else if ((Input.GetKeyDown(KeyCode.D) && PlayerID == 0) || (Input.GetKeyDown(KeyCode.K) && PlayerID == 1))
            {
                Debug.Log("spel down");
                SelectAction(Command.Spell, 1);
            }
            else if ((Input.GetKeyDown(KeyCode.A) && PlayerID == 0) || (Input.GetKeyDown(KeyCode.J) && PlayerID == 1))
            {
                Debug.Log("spel left");
                SelectAction(Command.Spell, 2);
            }
            else if ((Input.GetKeyDown(KeyCode.D) && PlayerID == 0) || (Input.GetKeyDown(KeyCode.L) && PlayerID == 1))
            {
                Debug.Log("spel right");
                SelectAction(Command.Spell, 3);
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
        RestoreMP(10);
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
        MP = Mathf.Clamp(MP, 0.0f, 100.0f);
        Health = Mathf.Clamp(Health, 0.0f, 100.0f);
        healthText.text = Health.ToString() + " / 100";
        mpText.text = MP.ToString() + " / 100";
        CommandUI.SetActive(true);
    }

    void SelectAction(Command selectedCommand, int selectedSpell)
    {
        Action = selectedCommand;
        if (Action == Command.Spell)
        {
            currentSpell = spellList[selectedSpell];
            if (MP >= currentSpell.GetCost())
            {
                MP -= currentSpell.GetCost();
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
        CommandUI.SetActive(false);
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
        Health = Mathf.Clamp(Health, 0.0f, 100.0f);
        healthText.text = Health.ToString() + " / 100";
        if(Health <= 0.0f)
        {
            PlayerDied();
        }
    }

    public void MagicCastSuccess()
    {
        mpText.text = MP.ToString() + " / 100";
    }

    void PlayerDied()
    {
        FC.PlayerDied(PlayerID);
    }

    void EndBattle()
    {
        
    }

    public void RestoreMP(float amt)
    {
        MP += amt;
        mpText.text = MP.ToString();
    }
}
