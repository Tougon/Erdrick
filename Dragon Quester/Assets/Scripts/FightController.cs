using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class FightController : MonoBehaviour
{

    [SerializeField] Player Player1, Player2;
    Player.Command P1Command, P2Command;
    Entity p1, p2;
    [SerializeField] GameObject battleUIElements, victory;
    [SerializeField] Text P1_CommandText, P2_CommandText;
    public List<Spell> SpellList;
    
    [SerializeField] int playersReady = 0;

    bool battling;
    public bool someoneDied;

    [SerializeField]
    List<AnimationSequenceObject> animations = new List<AnimationSequenceObject>();

    private void Awake()
    {
        p1 = Player1.GetComponent<Entity>();
        p2 = Player2.GetComponent<Entity>();
        BeginBattle();
    }

    void BeginBattle()
    {
        battling = true;
        playersReady = 0;
        Player1.StartBattle();
        Player2.StartBattle();
    }

    void ReceiveCommand()
    {
        Debug.Log("command get");
        playersReady++;
        if(playersReady == 2)
        {
            StartCoroutine(BeginTurn());
        }
    }

    IEnumerator BeginTurn()
    {
        yield return new WaitForSeconds(0.5f);
        TweenBattleUIIn();
        yield return new WaitForSeconds(0.8f);
        P1Command = Player1.GetAction();
        P2Command = Player2.GetAction();
        if(P1Command == Player.Command.Attack && P2Command == Player.Command.Attack)
        {
            P1_CommandText.text = "Attack!";
            P2_CommandText.text = "Attack!";

            StartAnimationSequence(animations[0], p1, p2);
            StartAnimationSequence(animations[0], p2, p1);

            yield return new WaitForSeconds(1.0f);
            Player1.TakeDamage(1);
            Player2.TakeDamage(1);

            StartAnimationSequence(animations[1], p1, p2);
            StartAnimationSequence(animations[1], p2, p1);

            P1_CommandText.text = "Deal 1 Damage!";
            P2_CommandText.text = "Deal 1 Damage!";
            Player1.RestoreMP(1);
            Player2.RestoreMP(1);
        }
        else if (P1Command == Player.Command.Attack && P2Command == Player.Command.Block)
        {
            P1_CommandText.text = "Attack!";
            P2_CommandText.text = "Block!";

            StartAnimationSequence(animations[0], p1, p2);
            StartAnimationSequence(animations[2], p2, p1);

            yield return new WaitForSeconds(1.0f);

            StartAnimationSequence(animations[3], p2, p1);

            Player1.TakeDamage(Player2.GetDamage());
            P1_CommandText.text = "Parried! No Damage!";
            P2_CommandText.text = ("Deal " + Player2.GetDamage().ToString() + " Damage!");
            Player2.RestoreMP(Player2.GetDamage() * 1.5f);
        }
        else if (P1Command == Player.Command.Attack && P2Command == Player.Command.Spell)
        {
            P1_CommandText.text = "Attack!";
            P2_CommandText.text = Player2.GetSpell().GetName();
            Player2.DrainMP(Player2.GetSpell().GetCost());
            
            StartAnimationSequence(animations[0], p1, p2);
            StartAnimationSequence(animations[4], p2, p1);

            yield return new WaitForSeconds(1.0f);
            Player2.TakeDamage(Player1.GetDamage());
            P1_CommandText.text = "Deal " + Player1.GetDamage() + " Damage!";
            P2_CommandText.text = "Interrupted!";
            Player1.RestoreMP(Player1.GetDamage());

            StartAnimationSequence(animations[1], p1, p2);
        }
        else if (P1Command == Player.Command.Block && P2Command == Player.Command.Attack)
        {
            P1_CommandText.text = "Block!";
            P2_CommandText.text = "Attack!";

            StartAnimationSequence(animations[2], p1, p2);
            StartAnimationSequence(animations[0], p2, p1);

            yield return new WaitForSeconds(1.0f);

            StartAnimationSequence(animations[3], p1, p2);

            Player2.TakeDamage(Player1.GetDamage());
            P1_CommandText.text = ("Deal " + Player1.GetDamage().ToString() + " Damage!");
            P2_CommandText.text = ("Parried! No Damage!");
            Player1.RestoreMP(Player1.GetDamage() * 1.5f);
        }
        else if (P1Command == Player.Command.Block && P2Command == Player.Command.Block)
        {
            P1_CommandText.text = "Block!";
            P2_CommandText.text = "Block!";

            StartAnimationSequence(animations[2], p1, p2);
            StartAnimationSequence(animations[2], p2, p1);

            yield return new WaitForSeconds(1.0f);
            P1_CommandText.text = "wow FUCKING nothing";
            P2_CommandText.text = "wow FUCKING nothing";
        }
        else if (P1Command == Player.Command.Block && P2Command == Player.Command.Spell)
        {
            P1_CommandText.text = "Block!";
            P2_CommandText.text = Player2.GetSpell().GetName();
            Player2.DrainMP(Player2.GetSpell().GetCost());

            StartAnimationSequence(animations[2], p1, p2);
            StartAnimationSequence(animations[4], p2, p1);

            yield return new WaitForSeconds(1.0f);
            Player1.TakeDamage(Player2.GetSpell().GetDamage());
            P1_CommandText.text = "wow FUCKING nothing";
            P2_CommandText.text = Player2.GetSpell().GetDescription();
            if (Player2.GetSpell().GetEffect() != null)
            {
                switch (Player2.GetSpell().GetEffect().onSelf)
                {
                    case true:
                        ApplyEffect(Player2, Player2.GetSpell().GetEffect());
                        break;
                    case false:
                        ApplyEffect(Player1, Player2.GetSpell().GetEffect());
                        break;
                }
            }

            StartAnimationSequence(Player2.GetSpell().GetAnimation(), p2, p1);
        }
        else if (P1Command == Player.Command.Spell && P2Command == Player.Command.Attack)
        {
            P1_CommandText.text = Player1.GetSpell().GetName();
            P2_CommandText.text = "Attack!";
            Player1.DrainMP(Player1.GetSpell().GetCost());

            StartAnimationSequence(animations[4], p1, p2);
            StartAnimationSequence(animations[0], p2, p1);

            yield return new WaitForSeconds(1.0f);
            Player1.TakeDamage(Player2.GetDamage());
            P1_CommandText.text = ("Interrupted!");
            P2_CommandText.text = ("Deal " + Player2.GetDamage().ToString() + " Damage!");
            Player2.RestoreMP(Player2.GetDamage());

            StartAnimationSequence(animations[1], p2, p1);
        }
        else if (P1Command == Player.Command.Spell && P2Command == Player.Command.Block)
        {
            P1_CommandText.text = Player1.GetSpell().GetName();
            P2_CommandText.text = "Block!";
            Player1.DrainMP(Player1.GetSpell().GetCost());
            
            StartAnimationSequence(animations[2], p1, p2);
            StartAnimationSequence(animations[4], p2, p1);

            yield return new WaitForSeconds(1.0f);
            Player2.TakeDamage(Player1.GetSpell().GetDamage());
            P1_CommandText.text = Player1.GetSpell().GetDescription();
            P2_CommandText.text = "wow FUCKING nothing";
            if (Player1.GetSpell().GetEffect() != null)
            {
                switch (Player1.GetSpell().GetEffect().onSelf)
                {
                    case true:
                        ApplyEffect(Player1, Player1.GetSpell().GetEffect());
                        break;
                    case false:
                        ApplyEffect(Player2, Player1.GetSpell().GetEffect());
                        break;
                }
            }

            StartAnimationSequence(Player1.GetSpell().GetAnimation(), p1, p2);
        }
        else if (P1Command == Player.Command.Spell && P2Command == Player.Command.Spell)
        {
            P1_CommandText.text = Player1.GetSpell().GetName();
            P2_CommandText.text = Player2.GetSpell().GetName();
            Player1.DrainMP(Player1.GetSpell().GetCost());
            Player2.DrainMP(Player2.GetSpell().GetCost());
            
            StartAnimationSequence(animations[4], p1, p2);
            StartAnimationSequence(animations[4], p2, p1);

            yield return new WaitForSeconds(1.0f);
            Player1.TakeDamage(Player2.GetSpell().GetDamage());
            Player2.TakeDamage(Player1.GetSpell().GetDamage());
            P1_CommandText.text = Player1.GetSpell().GetDescription();
            P2_CommandText.text = Player2.GetSpell().GetDescription();
            if (Player1.GetSpell().GetEffect() != null)
            {
                switch (Player1.GetSpell().GetEffect().onSelf)
                {
                    case true:
                        ApplyEffect(Player1, Player1.GetSpell().GetEffect());
                        break;
                    case false:
                        ApplyEffect(Player2, Player1.GetSpell().GetEffect());
                        break;
                }
            }
            if (Player2.GetSpell().GetEffect() != null)
            {
                switch (Player2.GetSpell().GetEffect().onSelf)
                {
                    case true:
                        ApplyEffect(Player2, Player2.GetSpell().GetEffect());
                        break;
                    case false:
                        ApplyEffect(Player1, Player2.GetSpell().GetEffect());
                        break;
                }
            }

            StartAnimationSequence(Player1.GetSpell().GetAnimation(), p1, p2);
            StartAnimationSequence(Player2.GetSpell().GetAnimation(), p2, p1);
        }

        else if (P1Command == Player.Command.None)
        {
            P1_CommandText.text = "Can't Act!";
            switch (P2Command)
            {
                case Player.Command.Attack:
                    P2_CommandText.text = "Attack!";

                    StartAnimationSequence(animations[0], p2, p1);

                    yield return new WaitForSeconds(1.0f);
                    Player1.TakeDamage(Player2.GetDamage());
                    P2_CommandText.text = ("Deal " + Player2.GetDamage().ToString() + " Damage!");
                    Player2.RestoreMP(Player2.GetDamage());

                    StartAnimationSequence(animations[1], p2, p1);

                    break;
                case Player.Command.Block:
                    P2_CommandText.text = "Block!";

                    StartAnimationSequence(animations[2], p2, p1);

                    yield return new WaitForSeconds(1.0f);
                    P2_CommandText.text = "wow FUCKING nothing";
                    break;
                case Player.Command.Spell:
                    P2_CommandText.text = Player2.GetSpell().GetName();
                    Player2.DrainMP(Player2.GetSpell().GetCost());

                    StartAnimationSequence(animations[4], p2, p1);

                    yield return new WaitForSeconds(1.0f);
                    Player1.TakeDamage(Player2.GetSpell().GetDamage());
                    P2_CommandText.text = Player2.GetSpell().GetDescription();
                    if (Player2.GetSpell().GetEffect() != null)
                    {
                        switch (Player2.GetSpell().GetEffect().onSelf)
                        {
                            case true:
                                ApplyEffect(Player2, Player2.GetSpell().GetEffect());
                                break;
                            case false:
                                ApplyEffect(Player1, Player2.GetSpell().GetEffect());
                                break;
                        }
                    }
                    StartAnimationSequence(Player2.GetSpell().GetAnimation(), p2, p1);
                    break;
                case Player.Command.None:
                    P2_CommandText.text = "Can't Act!";
                    yield return new WaitForSeconds(1.0f);
                    P1_CommandText.text = "wow FUCKING nothing";
                    P2_CommandText.text = "wow FUCKING nothing";
                    break;
                default:
                    Debug.LogError("uhhhhhh");
                    break;
            }
        }
        else if (P2Command == Player.Command.None)
        {
            P2_CommandText.text = "Can't Act!";
            switch (P1Command)
            {
                case Player.Command.Attack:
                    P1_CommandText.text = "Attack!";

                    StartAnimationSequence(animations[0], p1, p2);

                    yield return new WaitForSeconds(1.0f);
                    
                    StartAnimationSequence(animations[1], p1, p2);

                    Player2.TakeDamage(Player1.GetDamage());
                    P1_CommandText.text = ("Deal " + Player1.GetDamage().ToString() + " Damage!");
                    Player1.RestoreMP(Player1.GetDamage());
                    break;
                case Player.Command.Block:
                    P1_CommandText.text = "Block!";

                    StartAnimationSequence(animations[2], p1, p2);

                    yield return new WaitForSeconds(1.0f);

                    P1_CommandText.text = "wow FUCKING nothing";
                    break;
                case Player.Command.Spell:
                    P1_CommandText.text = Player1.GetSpell().GetName();
                    Player1.DrainMP(Player1.GetSpell().GetCost());
                    
                    StartAnimationSequence(animations[4], p1, p2);

                    yield return new WaitForSeconds(1.0f);
                    Player2.TakeDamage(Player1.GetSpell().GetDamage());
                    P1_CommandText.text = Player1.GetSpell().GetDescription();
                    if (Player1.GetSpell().GetEffect() != null)
                    {
                        switch (Player1.GetSpell().GetEffect().onSelf)
                        {
                            case true:
                                ApplyEffect(Player1, Player1.GetSpell().GetEffect());
                                break;
                            case false:
                                ApplyEffect(Player2, Player1.GetSpell().GetEffect());
                                break;
                        }
                    }

                    StartAnimationSequence(Player1.GetSpell().GetAnimation(), p1, p2);
                    break;
                case Player.Command.None:
                    P1_CommandText.text = "Can't Act!";
                    yield return new WaitForSeconds(1.0f);
                    P1_CommandText.text = "wow FUCKING nothing";
                    P2_CommandText.text = "wow FUCKING nothing";
                    break;
                default:
                    Debug.LogError("uhhhhhh");
                    break;
            }
        }
        else
        {
            Debug.LogError("bruh someboddy didn't pick either attack, block, or a spell ya dingus");
        }
        StartCoroutine(EndTurnTimer());
    }

    void EndTurn()
    {
        playersReady = 0;
        if (battling)
        {
            Player1.EndTurn(animations[5], p1);
            Player2.EndTurn(animations[5], p2);
        }
        else
        {
            // victory stuff
        }
    }

    public void PlayerDied(int whomst)
    {
        Debug.Log("died");
        //battling = false;
        EndBattle();
        if (!someoneDied)
        {
            Debug.Log("a" + whomst);
            someoneDied = true;
            switch (whomst)
            {
                case 0:
                    victory.GetComponentInChildren<Text>().text = "Player 2 Wins!";
                    Debug.Log(2);
                    break;
                case 1:
                    Debug.Log(1);
                    victory.GetComponentInChildren<Text>().text = "Player 1 Wins!";
                    break;
            }
        }
        else {
            victory.GetComponentInChildren<Text>().text = "Draw!";
            Debug.Log("nobody");
        }
        StartCoroutine(EndGameTimer());
    }

    void EndBattle()
    {
        battling = false;
        Player1.HideUIGameEnd();
        Player2.HideUIGameEnd();
    }

    IEnumerator EndTurnTimer()
    {
        yield return new WaitForSeconds(1.5f);
        TweenBattleUIOut();
        yield return new WaitForSeconds(0.5f);
        EndTurn();
    }

    IEnumerator EndGameTimer()
    {
        yield return new WaitForSeconds(1.0f);
        victory.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene(0);
    }

    public Spell GetRandomSpell()
    {
        return SpellList[Random.Range(0, SpellList.Count)];
    }

    public void RemoveSpell(Spell s)
    {
        SpellList.Remove(s);
    }

    public void AddSpell(Spell s)
    {
        SpellList.Add(s);
    }

    void ApplyEffect(Player p, Effect e)
    {
        p.AddEffect(e);
    }


    private void StartAnimationSequence(AnimationSequenceObject aso, Entity e1, Entity e2)
    {
        AnimationSequence seq = new AnimationSequence(aso, e1, e2);
        seq.SequenceStart();
        StartCoroutine(seq.SequenceLoop());
    }

    void TweenBattleUIIn()
    {
        P1_CommandText.text = "";
        P2_CommandText.text = "";
        battleUIElements.transform.DOMoveY(225.0f, 0.6f, true);
    }

    void TweenBattleUIOut()
    {
        battleUIElements.transform.DOMoveY(400, 0.6f, true);
    }
}
