using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightController : MonoBehaviour
{

    [SerializeField] Player Player1, Player2;
    Player.Command P1Command, P2Command;
    [SerializeField] GameObject battleUIElements;
    [SerializeField] Text P1_CommandText, P2_CommandText;
    
    int playersReady = 0;

    void BeginBattle()
    {
        playersReady = 0;
    }

    void ReceiveCommand()
    {
        playersReady++;
        if(playersReady == 2)
        {
            BeginTurn();
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
            P1_CommandText.text = "1 Damage!";
            P2_CommandText.text = "1 Damage!";
        }
        else if (P1Command == Player.Command.Attack && P2Command == Player.Command.Block)
        {
            P1_CommandText.text = "Attack!";
            P2_CommandText.text = "Block!";
            yield return new WaitForSeconds(1.0f);
            Player1.TakeDamage(Player2.GetDamage());
            P1_CommandText.text = (Player2.GetDamage().ToString() + " Damage!");
            P2_CommandText.text = "Counterattack!";
        }
        else if (P1Command == Player.Command.Attack && P2Command == Player.Command.Spell)
        {
            P1_CommandText.text = "Attack!";
            P2_CommandText.text = Player2.GetSpell().GetName();
            yield return new WaitForSeconds(1.0f);
            Player2.TakeDamage(Player1.GetDamage());
            P1_CommandText.text = "Interrupt!";
            P2_CommandText.text = (Player1.GetSpell().GetDamage().ToString() + " Damage!");
        }
        else if (P1Command == Player.Command.Block && P2Command == Player.Command.Attack)
        {
            P1_CommandText.text = "Block!";
            P2_CommandText.text = "Attack!";
            yield return new WaitForSeconds(1.0f);
            Player2.TakeDamage(Player1.GetDamage());
            P1_CommandText.text = "Counterattack!";
            P2_CommandText.text = (Player1.GetDamage().ToString() + " Damage!");
        }
        else if (P1Command == Player.Command.Block && P2Command == Player.Command.Block)
        {
            P1_CommandText.text = "Block!";
            P2_CommandText.text = "Block!";
            yield return new WaitForSeconds(1.0f);
        }
        else if (P1Command == Player.Command.Block && P2Command == Player.Command.Spell)
        {
            P1_CommandText.text = "Block!";
            P2_CommandText.text = Player2.GetSpell().GetName();
            Player1.TakeDamage(Player2.GetSpell().GetDamage());
            P1_CommandText.text = (Player2.GetSpell().GetDamage().ToString() + " Damage!");
        }
        else if (P1Command == Player.Command.Spell && P2Command == Player.Command.Attack)
        {
            P1_CommandText.text = Player1.GetSpell().GetName();
            P2_CommandText.text = "Attack!";
            yield return new WaitForSeconds(1.0f);
            Player1.TakeDamage(Player2.GetDamage());
            P1_CommandText.text = (Player2.GetDamage().ToString() + " Damage!");
            P2_CommandText.text = "Interrupt!";
        }
        else if (P1Command == Player.Command.Spell && P2Command == Player.Command.Block)
        {
            P1_CommandText.text = Player1.GetSpell().GetName();
            P2_CommandText.text = "Block!";
            yield return new WaitForSeconds(1.0f);
            Player2.TakeDamage(Player1.GetSpell().GetDamage());
            P2_CommandText.text = (Player1.GetSpell().GetDamage().ToString() + " Damage!");
        }
        else if (P1Command == Player.Command.Spell && P2Command == Player.Command.Spell)
        {
            P1_CommandText.text = Player1.GetSpell().GetName();
            P2_CommandText.text = Player2.GetSpell().GetName();
            yield return new WaitForSeconds(1.0f);
            Player1.TakeDamage(Player2.GetSpell().GetDamage());
            Player2.TakeDamage(Player1.GetSpell().GetDamage());
            P1_CommandText.text = (Player2.GetSpell().GetDamage().ToString() + " Damage!");
            P2_CommandText.text = (Player1.GetSpell().GetDamage().ToString() + " Damage!");
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
        Player1.StartTurn();
        Player2.StartTurn();
    }

    void PlayerDied(int whomst)
    {
        EndBattle();
    }

    void EndBattle()
    {

    }

    IEnumerator EndTurnTimer()
    {
        yield return new WaitForSeconds(2.0f);
        EndTurn();
    }
}
