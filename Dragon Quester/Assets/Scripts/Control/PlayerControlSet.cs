using System.Collections;
using System.Collections.Generic;
using InControl;

/// <summary>
/// Action set for player controls
/// </summary>
public class PlayerControlSet : PlayerActionSet
{
    public PlayerAction SelectAttack;
    public PlayerAction SelectBlock;
    public PlayerAction SelectSpellUp;
    public PlayerAction SelectSpellDown;
    public PlayerAction SelectSpellLeft;
    public PlayerAction SelectSpellRight;


    public PlayerControlSet()
    {
        SelectAttack = CreatePlayerAction("Attack");
        SelectBlock = CreatePlayerAction("Block");
        SelectSpellUp = CreatePlayerAction("SpellUp");
        SelectSpellDown = CreatePlayerAction("SpellDown");
        SelectSpellLeft = CreatePlayerAction("SpellLeft");
        SelectSpellRight = CreatePlayerAction("SpellRight");
    }


    public void InitKeyboardcontrols(int val, bool P2NumPad)
    {
        if (val == 1)
            InitP1KeyboardControls();
        else
            if (P2NumPad)
            {
                InitP2NumpadControls();
            }
            else
            {
            InitP2KeyboardControls();
            }
    }


    public void InitP1KeyboardControls()
    {
        SelectAttack.AddDefaultBinding(Key.Q);
        SelectBlock.AddDefaultBinding(Key.E);
        SelectSpellUp.AddDefaultBinding(Key.W);
        SelectSpellDown.AddDefaultBinding(Key.S);
        SelectSpellLeft.AddDefaultBinding(Key.A);
        SelectSpellRight.AddDefaultBinding(Key.D);
    }


    public void InitP2KeyboardControls()
    {
        SelectAttack.AddDefaultBinding(Key.U);
        SelectBlock.AddDefaultBinding(Key.O);
        SelectSpellUp.AddDefaultBinding(Key.I);
        SelectSpellDown.AddDefaultBinding(Key.J);
        SelectSpellLeft.AddDefaultBinding(Key.K);
        SelectSpellRight.AddDefaultBinding(Key.L);
    }

    public void InitP2NumpadControls()
    {
        SelectAttack.AddDefaultBinding(Key.Pad7);
        SelectBlock.AddDefaultBinding(Key.Pad9);
        SelectSpellUp.AddDefaultBinding(Key.Pad8);
        SelectSpellDown.AddDefaultBinding(Key.Pad5);
        SelectSpellLeft.AddDefaultBinding(Key.Pad4);
        SelectSpellRight.AddDefaultBinding(Key.Pad6);
    }
}
