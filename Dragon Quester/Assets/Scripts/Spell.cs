using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSpell", menuName = "ScriptableObjects/Spell", order = 1)]
public class Spell : ScriptableObject
{
    [SerializeField] string Spell_Name;
    [SerializeField] float Damage;
    [SerializeField] float MP_Cost;
    [SerializeField] string Description;
    [SerializeField] Effect effect;

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

    public string GetDescription()
    {
        return Description;
    }

    public Effect GetEffect()
    {
        return effect;
    }
}
