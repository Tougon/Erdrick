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

    void BeginTurn()
    {
        battleUIElements.SetActive(true);
        P1Command = Player1.GetAction();
        P2Command = Player2.GetAction();
        if(P1Command == Player.Command.Attack && P2Command == Player.Command.Attack)
        {
            P1_CommandText.text = "Attack!";
            P2_CommandText.text = "Attack!";
        }
        else if (P1Command == Player.Command.Attack && P2Command == Player.Command.Block)
        {
            P1_CommandText.text = "Attack!";
            P2_CommandText.text = "Block!";
        }
        else if (P1Command == Player.Command.Attack && P2Command == Player.Command.Spell)
        {
            P1_CommandText.text = "Attack!";
            P2_CommandText.text = Player2.GetSpellName();
        }
        else if (P1Command == Player.Command.Block && P2Command == Player.Command.Attack)
        {
            P1_CommandText.text = "Block!";
            P2_CommandText.text = "Attack!";
        }
        else if (P1Command == Player.Command.Block && P2Command == Player.Command.Block)
        {
            P1_CommandText.text = "Block!";
            P2_CommandText.text = "Block!";
        }
        else if (P1Command == Player.Command.Block && P2Command == Player.Command.Spell)
        {
            P1_CommandText.text = "Block!";
            P2_CommandText.text = Player2.GetSpellName();
        }
        else if (P1Command == Player.Command.Spell && P2Command == Player.Command.Attack)
        {
            P1_CommandText.text = Player1.GetSpellName();
            P2_CommandText.text = "Attack!";
        }
        else if (P1Command == Player.Command.Spell && P2Command == Player.Command.Block)
        {
            P1_CommandText.text = Player1.GetSpellName();
            P2_CommandText.text = "Block!";
        }
        else if (P1Command == Player.Command.Spell && P2Command == Player.Command.Spell)
        {
            P1_CommandText.text = Player1.GetSpellName();
            P2_CommandText.text = Player2.GetSpellName();
        }
        StartCoroutine(EndTurnTimer());
    }

    void EndTurn()
    {
        battleUIElements.SetActive(false);
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
        yield return new WaitForSeconds(3.0f);
        EndTurn();
    }
}
