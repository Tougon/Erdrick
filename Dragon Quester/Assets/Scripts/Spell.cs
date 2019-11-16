using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSpell", menuName = "ScriptableObjects/Spell", order = 1)]
public class Spell : ScriptableObject
{
    [SerializeField] float MP_Cost;
    [SerializeField] string Spell_Name;
    [SerializeField] float Damage;

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
