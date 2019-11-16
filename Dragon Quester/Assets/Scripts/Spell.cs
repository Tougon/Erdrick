﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : ScriptableObject
{
    float MP_Cost;
    string Spell_Name;
    float Damage;

    public float GetCost()
    {
        return MP_Cost;
    }

    public string GetName()
    {
        return Spell_Name;
    }

    public float GetDamage()
    {
        return Damage;
    }
}
