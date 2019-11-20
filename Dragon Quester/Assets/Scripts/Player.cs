using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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

    bool canDoThings, shouldRestoreMP, alreadyDead;

    Animator anim;
    PlayerControlSet controls;

    [SerializeField] List<Effect> currentEffects;

    [SerializeField] private bool P2NumPadControls;

    [SerializeField] Image SpellImage, AtkImage, BlkImage;
    [SerializeField] Sprite NumSpell, NumAtk, NumBlk;

    [SerializeField] float MPRestoreAmount;

    private void Awake()
    {
        //canDoThings = false;
        anim = GetComponent<Animator>();

        alreadyDead = false;

        controls = new PlayerControlSet();
        controls.InitKeyboardcontrols(PlayerID+1, P2NumPadControls);
        if(P2NumPadControls && PlayerID == 1)
        {
            SpellImage.sprite = NumSpell;
            AtkImage.sprite = NumAtk;
            BlkImage.sprite = NumBlk;
        }
        DOTween.Init();
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
            RestoreMP(MPRestoreAmount);
        shouldRestoreMP = true;
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
                    status.RemoveEffectFromList(currentEffects[i]);
                    currentEffects.Remove(currentEffects[i]);
                    i--;
                }
                else if (currentEffects[i].turns <= -1)
                {
                    status.RemoveEffectFromList(currentEffects[i]);
                    currentEffects[i].ActivateEffect();
                    currentEffects.Remove(currentEffects[i]);
                    i--;
                }
            }
        }
        if (!FC.someoneDied)
        {
            canDoThings = true;
            if (canDoThings)
                ShowUI();
        }
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
            MP = Mathf.Clamp(MP, 0.0f, 100.0f);
            Health = Mathf.Clamp(Health, 0.0f, 100.0f);
            healthText.text = Health.ToString() + " / 100 HP";
            mpText.text = MP.ToString() + " / 100 MP";
            TweenUIIn();
        }
    }

    void TweenUIIn()
    {
        //CommandUI.SetActive(true);
        CommandUI.transform.DOLocalMoveY(0.0f, 1.0f, true);
    }

    void TweenUIOut()
    {
        CommandUI.transform.DOLocalMoveY(-300.0f, 0.5f, true);
        //CommandUI.SetActive(false);
    }

    void SelectAction(Command selectedCommand, int selectedSpell)
    {
        if (canDoThings)
        {
            SoundManager.Instance.PlaySound("Sounds/move_select");
        }
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
        foreach (Effect e in currentEffects)
        {
            status.RemoveEffectFromList(e);
        }
        TweenUIOut();
        canDoThings = false;
    }

    void HideUICommand()
    {
        // hide the ui here and the player can't do anything
        TweenUIOut();
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
        if (Health < 100)
        {
            if (Health > 90) healing = 100 - Health;
            Health += healing;
            Health = Mathf.Clamp(Health, 0.0f, 100.0f);
            healthText.text = Health.ToString() + " / 100 HP";
            ScaleHealthBar();
            GameObject newPopup = Instantiate(HP_Pop, HP_Pos.position, Quaternion.identity, GameObject.Find("Canvas").GetComponent<RectTransform>()).gameObject;
            newPopup.GetComponentInChildren<Text>().text = ("+" + healing);
        }

        SoundManager.Instance.PlaySound("Sounds/heal");
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
        HealthBar.transform.DOScaleX(0.01f * Health, 0.3f);
    }

    void ScaleMPBar()
    {
        MPBar.transform.DOScaleX(0.01f * MP, 0.3f);
        //MPBar.sizeDelta = new Vector2(MP * 10.0f, 50.0f);
    }

    void PlayerDied()
    {
        CommandUI.SetActive(false);
        TweenUIOut();
        bool kamikazee = false;

        foreach(Effect e in currentEffects)
        {
            if (e.endTurnDamage) kamikazee = true;
            status.RemoveEffectFromList(e);
        }
        if (!alreadyDead)
        {
            Debug.Log("i died " + PlayerID);
            alreadyDead = true;
            FC.PlayerDied(PlayerID, kamikazee);
        }
    }

    public void EndTurn(AnimationSequenceObject aso, Entity entity, AnimationSequenceObject restore)
    {
        bool isDead = false;
        int numSnooze = 0;

        if (currentEffects.Count >= 1)
        {
            for(int i = 0; i < currentEffects.Count; i++)
            {
                if (currentEffects[i].gameObject.name.Contains("_Heal"))
                {
                    currentEffects[i].ActivateEffect();
                }
                if (currentEffects[i].gameObject.name.Contains("Kamikazee"))
                {
                    isDead = true;
                    currentEffects[i].ActivateEffect();
                }
                
                if (currentEffects[i].gameObject.name.Contains("Snooze"))
                    numSnooze++;

                if (currentEffects[i].turns <= 0)
                {
                    if (currentEffects[i].gameObject.name.Contains("Snooze"))
                        numSnooze--;

                    status.RemoveEffectFromList(currentEffects[i]);
                    currentEffects.Remove(currentEffects[i]);
                    i--;
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

    public void EndBattle()
    {

        TweenUIOut();
        canDoThings = false;
        foreach (Effect e in currentEffects)
        {
            status.RemoveEffectFromList(e);
        }
    }

    public void SetAnimation(string val)
    {
        anim.SetTrigger(val);
    }

    public void RestoreMP(float amt)
    {
        if (MP < 100)
        {
            if (MP > (100 - MPRestoreAmount)) amt = 100 - MP;
            MP += amt;
            MP = Mathf.Clamp(MP, 0.0f, 100.0f);
            mpText.text = MP.ToString() + " / 100 MP";
            GameObject newPopup = Instantiate(MP_Pop, MP_Pos.position, Quaternion.identity, GameObject.Find("Canvas").GetComponent<RectTransform>()).gameObject;
            newPopup.GetComponentInChildren<Text>().text = ("+" + amt);
            ScaleMPBar();
        }
    }

    public void DrainMP(float amt)
    {
        if (amt != 0)
        {
            MP -= amt;
            MP = Mathf.Clamp(MP, 0.0f, 100.0f);
            mpText.text = MP.ToString() + " / 100 MP";
            GameObject newPopup = Instantiate(MP_Pop, MP_Pos.position, Quaternion.identity, GameObject.Find("Canvas").GetComponent<RectTransform>()).gameObject;
            newPopup.GetComponentInChildren<Text>().text = ("-" + amt);
            ScaleMPBar();
        }
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

    public float GetHealth()
    {
        return Health;
    }
}
