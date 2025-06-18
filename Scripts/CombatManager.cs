using UnityEngine;
using System.Collections.Generic;

public class CombatManager
{
    public void StartCombat(Force attacker, Force defender)
    {
        while (attacker.units.Count > 0 && defender.units.Count > 0)
        {
            int attackerDamage = CalculateDamage(attacker);
            int defenderDefense = CalculateDefense(defender);

            if (attackerDamage > defenderDefense)
            {
                int damageTaken = attackerDamage - defenderDefense;
                defender.TakeDamage(damageTaken);
            }

            // Контратака
            if (defender.units.Count > 0)
            {
                int defenderDamage = CalculateDamage(defender);
                int attackerDefense = CalculateDefense(attacker);

                if (defenderDamage > attackerDefense)
                {
                    int damageTaken = defenderDamage - attackerDefense;
                    attacker.TakeDamage(damageTaken);
                }
            }
        }
    }

    private int CalculateDamage(Force force)
    {
        int totalDamage = 0;
        foreach (Unit unit in force.units)
        {
            if (unit.definition.role != UnitRole.Worker && unit.definition.role != UnitRole.Engineer && unit.definition.role != UnitRole.Diplomat && unit.definition.role != UnitRole.Spy)
            {
                int unitDamage = unit.definition.attack;
                unitDamage = (int)(unitDamage * (0.7f + 0.3f * Random.Range(1, 7)));
                totalDamage += unitDamage;
            }
        }
        return totalDamage;
    }

    private int CalculateDefense(Force force)
    {
        int totalDefense = 0;
        foreach (Unit unit in force.units)
        {
            if (unit.definition.role != UnitRole.Worker && unit.definition.role != UnitRole.Engineer && unit.definition.role != UnitRole.Diplomat && unit.definition.role != UnitRole.Spy)
            {
                int unitDefense = unit.definition.defense;
                unitDefense = (int)(unitDefense * (0.7f + 0.3f * Random.Range(1, 7)));
                totalDefense += unitDefense;
            }
        }
        return totalDefense;
    }
}

public static class ForceExtensions
{
    public static void TakeDamage(this Force force, int damage)
    {
        while (damage > 0 && force.units.Count > 0)
        {
            int index = Random.Range(0, force.units.Count);
            force.units.RemoveAt(index);
            damage--;
        }
    }
}
