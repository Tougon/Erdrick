using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightController : MonoBehaviour
{

    [SerializeField] Player Player1, Player2;
    Player.Command P1Command, P2Command;
    
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
        P1Command = Player1.GetAction();
        P2Command = Player2.GetAction();
        if(P1Command == Player.Command.Attack && P2Command == Player.Command.Attack)
        {
            // both take some damage, not full damage
        }
        else if (P1Command == Player.Command.Attack && P2Command == Player.Command.Block)
        {
            // p2 blocks, counterattacks p1
        }
        else if (P1Command == Player.Command.Attack && P2Command == Player.Command.Spell)
        {
            // p1 attacks, p2 does not cast spell
        }
        else if (P1Command == Player.Command.Block && P2Command == Player.Command.Attack)
        {
            // p1 blocks, counterattacks p2
        }
        else if (P1Command == Player.Command.Block && P2Command == Player.Command.Block)
        {
            // nothing happens
        }
        else if (P1Command == Player.Command.Block && P2Command == Player.Command.Spell)
        {
            // p2 spells p1
        }
        else if (P1Command == Player.Command.Spell && P2Command == Player.Command.Attack)
        {
            // p2 attacks, p1 does not cast spell
        }
        else if (P1Command == Player.Command.Spell && P2Command == Player.Command.Block)
        {
            // p1 spells p2
        }
        else if (P1Command == Player.Command.Spell && P2Command == Player.Command.Spell)
        {
            // p1 spells p2, p2 spells p1
        }
    }

    void EndTurn()
    {
        playersReady = 0;
    }

    void PlayerDied(int whomst)
    {
        EndBattle();
    }

    void EndBattle()
    {

    }
}
