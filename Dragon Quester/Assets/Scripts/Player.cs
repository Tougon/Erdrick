using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] FightController FC;
    [SerializeField] int PlayerID;
    [SerializeField] GameObject CommandUI;
    [SerializeField] Text healthText, mpText, spellUp, spellDown, spellLeft, spellRight;
    [SerializeField] RectTransform HealthBar, MPBar;

    [HideInInspector] public enum Command { Attack, Block, Spell };

    float Health, MP;

    float atkDamage;

    Command Action;
    Spell currentSpell;
    [SerializeField] List<Spell> spellList;

    bool canDoThings;

    Animator anim;
    PlayerControlSet controls;

    private void Awake()
    {
        canDoThings = false;
        anim = GetComponent<Animator>();

        controls = new PlayerControlSet();
        controls.InitKeyboardcontrols(PlayerID+1);
    }

    private void Update()
    {
        #region Garbage Input Test Stuff
        if (canDoThings)
        {
            if (controls.SelectAttack.WasPressed)
            {
                Debug.Log("attak");
                SelectAction(Command.Attack, 0);
            }
            else if (controls.SelectBlock.WasPressed)
            {
                Debug.Log("blok");
                SelectAction(Command.Block, 0);
            }
            else if (controls.SelectSpellUp.WasPressed)
            {
                Debug.Log("spel up");
                SelectAction(Command.Spell, 0);
            }
            else if (controls.SelectSpellDown.WasPressed)
            {
                Debug.Log("spel down");
                SelectAction(Command.Spell, 1);
            }
            else if (controls.SelectSpellLeft.WasPressed)
            {
                Debug.Log("spel left");
                SelectAction(Command.Spell, 2);
            }
            else if (controls.SelectSpellRight.WasPressed)
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
        ScaleHealthBar();
        if(Health <= 0.0f)
        {
            PlayerDied();
        }
    }

    void ScaleHealthBar()
    {
        HealthBar.sizeDelta = new Vector2(Health * 10.0f, 50.0f);
    }

    void ScaleMPBar()
    {
        MPBar.sizeDelta = new Vector2(MP * 10.0f, 50.0f);
    }

    void PlayerDied()
    {
        FC.PlayerDied(PlayerID);
    }

    void EndBattle()
    {
        
    }

    public void SetAnimation(string val)
    {
        anim.SetTrigger(val);
    }

    public void RestoreMP(float amt)
    {
        MP += amt;
        MP = Mathf.Clamp(MP, 0.0f, 100.0f);
        mpText.text = MP.ToString() + " / 100";
        ScaleMPBar();
    }

    public void DrainMP(float amt)
    {
        MP -= amt;
        MP = Mathf.Clamp(MP, 0.0f, 100.0f);
        mpText.text = MP.ToString() + " / 100";
        ScaleMPBar();
    }
}
