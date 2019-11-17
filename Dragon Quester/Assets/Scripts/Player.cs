﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] FightController FC;
    [SerializeField] int PlayerID;
    [SerializeField] GameObject CommandUI;
    [SerializeField] Text healthText, mpText, spellUp, spellDown, spellLeft, spellRight;
    [SerializeField] RectTransform HealthBar, MPBar, HP_Pop, MP_Pop, HP_Pos, MP_Pos;

    [SerializeField] Status_Popup status;

    [HideInInspector] public enum Command { Attack, Block, Spell, None };

    float Health, MP, atkDamage;

    Command Action;
    Spell currentSpell;
    [SerializeField] List<Spell> spellList;

    bool canDoThings, shouldRestoreMP;

    Animator anim;
    PlayerControlSet controls;

    [SerializeField] List<Effect> currentEffects;

    private void Awake()
    {
        //canDoThings = false;
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
        currentEffects = new List<Effect>();
        shouldRestoreMP = false;
        atkDamage = 10.0f;
        Health = 100.0f;
        MP = 100.0f;
        StartTurn();
    }

    public void StartTurn()
    {
        FillSpellList();
        if (shouldRestoreMP)
            RestoreMP(10);
        shouldRestoreMP = true;
        canDoThings = true;
        if (currentEffects.Count >= 1)
        {
            for (int i = 0; i < currentEffects.Count; i++)
            {
                if (currentEffects[i].turns >= 1)
                {
                    currentEffects[i].ActivateEffect();
                    currentEffects[i].turns--;
                }
                else if (currentEffects[i].turns == 0)
                {
                    currentEffects.Remove(currentEffects[i]);
                    i--;
                }
                else if (currentEffects[i].turns <= -1)
                {
                    currentEffects[i].ActivateEffect();
                    currentEffects.Remove(currentEffects[i]);
                    i--;
                }
            }
        }
        if(canDoThings)
            ShowUI();
    }

    void FillSpellList()
    {
        for(int i = 0; i < spellList.Count; i++)
        {
            spellList[i] = FC.GetRandomSpell();
            FC.RemoveSpell(spellList[i]);
        }
        spellUp.text = spellList[0].GetName() + " (" + spellList[0].GetCost().ToString() + " MP)";
        spellDown.text = spellList[1].GetName() + " (" + spellList[1].GetCost().ToString() + " MP)";
        spellLeft.text = spellList[2].GetName() + " (" + spellList[2].GetCost().ToString() + " MP)";
        spellRight.text = spellList[3].GetName() + " (" + spellList[3].GetCost().ToString() + " MP)";
        for (int i = 0; i < spellList.Count; i++)
        {
            FC.AddSpell(spellList[i]);
        }
    }

    void ShowUI()
    {
        if (!FC.someoneDied)
        {
            Debug.Log("b" + PlayerID);
            MP = Mathf.Clamp(MP, 0.0f, 100.0f);
            Health = Mathf.Clamp(Health, 0.0f, 100.0f);
            healthText.text = Health.ToString() + " / 100 HP";
            mpText.text = MP.ToString() + " / 100 MP";
            CommandUI.SetActive(true);
        }
    }

    void SelectAction(Command selectedCommand, int selectedSpell)
    {
        Action = selectedCommand;
        if (Action == Command.Spell)
        {
            currentSpell = spellList[selectedSpell];
            if (MP >= currentSpell.GetCost())
            {
                HideUICommand();
            }
            else
            {
                // player didn't have enough mp, don't do anything
            }
        }
        else
        {
            HideUICommand();
        }
    }

    public void HideUIGameEnd()
    {
        CommandUI.SetActive(false);
        canDoThings = false;
    }

    void HideUICommand()
    {
        // hide the ui here and the player can't do anything
        CommandUI.SetActive(false);
        canDoThings = false;
        SendCommand();
    }

    public void SendCommand()
    {
        if (!FC) FC = GameObject.Find("FightController").GetComponent<FightController>();
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

    public void Heal(float healing)
    {
        Health += healing;
        Health = Mathf.Clamp(Health, 0.0f, 100.0f);
        healthText.text = Health.ToString() + " / 100 HP";
        ScaleHealthBar();
        GameObject newPopup = Instantiate(HP_Pop, HP_Pos.position, Quaternion.identity, GameObject.Find("Canvas").GetComponent<RectTransform>()).gameObject;
        newPopup.GetComponentInChildren<Text>().text = ("+" + healing);
    }

    public void TakeDamage(float damage)
    {
        if (damage != 0)
        {
            Health -= damage;
            Health = Mathf.Clamp(Health, 0.0f, 100.0f);
            healthText.text = Health.ToString() + " / 100 HP";
            ScaleHealthBar();
            GameObject newPopup = Instantiate(HP_Pop, HP_Pos.position, Quaternion.identity, GameObject.Find("Canvas").GetComponent<RectTransform>()).gameObject;
            newPopup.GetComponentInChildren<Text>().text = ("-" + damage);
            CheckAlive();
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
        foreach(Effect e in currentEffects)
        {
            status.RemoveEffectFromList(e);
        }
        Debug.Log("i died " + PlayerID);
        FC.PlayerDied(PlayerID);
    }

    public void EndTurn(AnimationSequenceObject aso, Entity entity, AnimationSequenceObject restore)
    {
        bool isDead = false;
        int numSnooze = 0;

        if (currentEffects.Count >= 1)
        {
            for(int i = 0; i < currentEffects.Count; i++)
            {
                if (currentEffects[i].gameObject.name.Contains("Kamikazee"))
                    isDead = true;
                
                if (currentEffects[i].gameObject.name.Contains("Snooze"))
                    numSnooze++;

                if (currentEffects[i].turns <= 0)
                {
                    if (currentEffects[i].gameObject.name.Contains("Snooze"))
                        numSnooze--;

                    currentEffects.Remove(currentEffects[i]);
                    i--;

                if (currentEffects[i].turns <= 0)
                {
                    status.RemoveEffectFromList(currentEffects[i]);
                }
            }
        }
        //CheckAlive();

        if(numSnooze <= 0)
        {
            AnimationSequence seq = new AnimationSequence(restore, entity, null);
            seq.SequenceStart();
            StartCoroutine(seq.SequenceLoop());
        }


        if(Health > 0 && !isDead)
        {
            AnimationSequence seq = new AnimationSequence(aso, entity, null);
            seq.SequenceStart();
            StartCoroutine(seq.SequenceLoop());
        }

        StartTurn();
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
        mpText.text = MP.ToString() + " / 100 MP";
        GameObject newPopup = Instantiate(MP_Pop, MP_Pos.position, Quaternion.identity, GameObject.Find("Canvas").GetComponent<RectTransform>()).gameObject;
        newPopup.GetComponentInChildren<Text>().text = ("+" + amt);
        ScaleMPBar();
    }

    public void DrainMP(float amt)
    {
        MP -= amt;
        MP = Mathf.Clamp(MP, 0.0f, 100.0f);
        mpText.text = MP.ToString() + " / 100 MP";
        GameObject newPopup = Instantiate(MP_Pop, MP_Pos.position, Quaternion.identity, GameObject.Find("Canvas").GetComponent<RectTransform>()).gameObject;
        newPopup.GetComponentInChildren<Text>().text = ("-" + amt);
        ScaleMPBar();
    }

    public void CantAct()
    {
        Debug.Log("cant move");
        canDoThings = false;
        SelectAction(Command.None, 0);
        Debug.Log("shouldnt act " + PlayerID);
    }

    public void AddEffect(Effect effect)
    {
        currentEffects.Add(Instantiate(effect));
        currentEffects[currentEffects.Count - 1].ApplyEffect(gameObject.GetComponent<Player>());
        status.AddEffectToList(effect);
    }

    void CheckAlive()
    {
        if(Health <= 0)
        {
            Debug.Log("died");
            PlayerDied();
        }
    }
}
