using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FightController : MonoBehaviour
{

    [SerializeField] Player Player1, Player2;
    Player.Command P1Command, P2Command;
    [SerializeField] GameObject battleUIElements, victory;
    [SerializeField] Text P1_CommandText, P2_CommandText;
    public List<Spell> SpellList;
    
    int playersReady = 0;

    bool battling, someoneDied;

    private void Awake()
    {
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
        playersReady++;
        if(playersReady == 2)
        {
            StartCoroutine(BeginTurn());
        }
    }

    IEnumerator BeginTurn()
    {
        yield return new WaitForSeconds(1.0f);
        battleUIElements.SetActive(true);
        P1Command = Player1.GetAction();
        P2Command = Player2.GetAction();
        if(P1Command == Player.Command.Attack && P2Command == Player.Command.Attack)
        {
            P1_CommandText.text = "Attack!";
            P2_CommandText.text = "Attack!";
            yield return new WaitForSeconds(1.0f);
            Player1.TakeDamage(1);
            Player2.TakeDamage(1);
            P1_CommandText.text = "Took 1 Damage!";
            P2_CommandText.text = "Took 1 Damage!";
            Player1.RestoreMP(1);
            Player2.RestoreMP(1);
        }
        else if (P1Command == Player.Command.Attack && P2Command == Player.Command.Block)
        {
            P1_CommandText.text = "Attack!";
            P2_CommandText.text = "Block!";
            yield return new WaitForSeconds(1.0f);
            Player1.TakeDamage(Player2.GetDamage());
            P1_CommandText.text = ("Took " + Player2.GetDamage().ToString() + " Damage!");
            P2_CommandText.text = "Counterattack!";
            Player2.RestoreMP(Player2.GetDamage() * 1.5f);
        }
        else if (P1Command == Player.Command.Attack && P2Command == Player.Command.Spell)
        {
            P1_CommandText.text = "Attack!";
            P2_CommandText.text = Player2.GetSpell().GetName();
            Player2.DrainMP(Player2.GetSpell().GetCost());
            yield return new WaitForSeconds(1.0f);
            Player2.TakeDamage(Player1.GetDamage());
            P1_CommandText.text = "Interrupt!";
            P2_CommandText.text = ("Took " + Player1.GetSpell().GetDamage().ToString() + " Damage!");
            Player1.RestoreMP(Player1.GetDamage());
        }
        else if (P1Command == Player.Command.Block && P2Command == Player.Command.Attack)
        {
            P1_CommandText.text = "Block!";
            P2_CommandText.text = "Attack!";
            yield return new WaitForSeconds(1.0f);
            Player2.TakeDamage(Player1.GetDamage());
            P1_CommandText.text = "Counterattack!";
            P2_CommandText.text = ("Took " + Player1.GetDamage().ToString() + " Damage!");
            Player1.RestoreMP(Player1.GetDamage() * 1.5f);
        }
        else if (P1Command == Player.Command.Block && P2Command == Player.Command.Block)
        {
            P1_CommandText.text = "Block!";
            P2_CommandText.text = "Block!";
            yield return new WaitForSeconds(1.0f);
            P1_CommandText.text = "wow FUCKING nothing";
            P2_CommandText.text = "wow FUCKING nothing";
        }
        else if (P1Command == Player.Command.Block && P2Command == Player.Command.Spell)
        {
            P1_CommandText.text = "Block!";
            P2_CommandText.text = Player2.GetSpell().GetName();
            Player2.DrainMP(Player2.GetSpell().GetCost());
            yield return new WaitForSeconds(1.0f);
            Player1.TakeDamage(Player2.GetSpell().GetDamage());
            P1_CommandText.text = ("Took " + Player2.GetSpell().GetDamage().ToString() + " Damage!");
        }
        else if (P1Command == Player.Command.Spell && P2Command == Player.Command.Attack)
        {
            P1_CommandText.text = Player1.GetSpell().GetName();
            P2_CommandText.text = "Attack!";
            Player1.DrainMP(Player1.GetSpell().GetCost());
            yield return new WaitForSeconds(1.0f);
            Player1.TakeDamage(Player2.GetDamage());
            P1_CommandText.text = ("Took " + Player2.GetDamage().ToString() + " Damage!");
            P2_CommandText.text = "Interrupt!";
            Player2.RestoreMP(Player2.GetDamage());
        }
        else if (P1Command == Player.Command.Spell && P2Command == Player.Command.Block)
        {
            P1_CommandText.text = Player1.GetSpell().GetName();
            P2_CommandText.text = "Block!";
            Player1.DrainMP(Player1.GetSpell().GetCost());
            yield return new WaitForSeconds(1.0f);
            Player2.TakeDamage(Player1.GetSpell().GetDamage());
            P2_CommandText.text = ("Took " + Player1.GetSpell().GetDamage().ToString() + " Damage!");
        }
        else if (P1Command == Player.Command.Spell && P2Command == Player.Command.Spell)
        {
            P1_CommandText.text = Player1.GetSpell().GetName();
            P2_CommandText.text = Player2.GetSpell().GetName();
            Player1.DrainMP(Player1.GetSpell().GetCost());
            Player2.DrainMP(Player2.GetSpell().GetCost());
            yield return new WaitForSeconds(1.0f);
            Player1.TakeDamage(Player2.GetSpell().GetDamage());
            Player2.TakeDamage(Player1.GetSpell().GetDamage());
            P1_CommandText.text = ("Took " + Player2.GetSpell().GetDamage().ToString() + " Damage!");
            P2_CommandText.text = ("Took " + Player1.GetSpell().GetDamage().ToString() + " Damage!");
        }
        else
        {
            Debug.LogError("bruh someboddy didn't pick either attack, block, or a spell ya dingus");
        }
        StartCoroutine(EndTurnTimer());
    }

    void EndTurn()
    {
        battleUIElements.SetActive(false);
        P1_CommandText.text = "test";
        P2_CommandText.text = "test";
        playersReady = 0;
        if (battling)
        {
            Player1.StartTurn();
            Player2.StartTurn();
        }
        else
        {
            // victory stuff
            victory.SetActive(true);
            StartCoroutine(EndGameTimer());
        }
    }

    public void PlayerDied(int whomst)
    {
        //battling = false;
        EndBattle();
        if (!someoneDied)
        {
            someoneDied = true;
            switch (whomst)
            {
                case 0:
                    victory.GetComponentInChildren<Text>().text = "Player 2 Wins!";
                    break;
                case 1:
                    victory.GetComponentInChildren<Text>().text = "Player 1 Wins!";
                    break;
            }
        }
        else {
            victory.GetComponentInChildren<Text>().text = "Draw!";
        }
    }

    void EndBattle()
    {
        battling = false;
    }

    IEnumerator EndTurnTimer()
    {
        yield return new WaitForSeconds(2.0f);
        EndTurn();
    }

    IEnumerator EndGameTimer()
    {
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene(0);
    }

    public Spell GetRandomSpell()
    {
        return SpellList[Random.Range(0, SpellList.Count)];
    }
}
