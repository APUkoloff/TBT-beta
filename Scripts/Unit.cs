using UnityEngine;

[System.Serializable]
public class Unit
{
    public string unitName;
    public UnitDefinition definition;
    public int currentMovePoints;
    public int health;
    public int interception; // Значение Interception для юнита

    public Unit(string name, UnitDefinition def)
    {
        unitName = name;
        definition = def;
        currentMovePoints = def.baseMovePoints;
        health = 1;
        interception = GetInterceptionValue();
    }

    // Метод для получения значения Interception
    private int GetInterceptionValue()
    {
        // Заглушка
        switch (definition.role)
        {
            case UnitRole.Flak:
                return 10;
            case UnitRole.Fighter:
                return 25;
            case UnitRole.SAM:
                return 50;
            default:
                return 1;
        }
    }

    // Метод для восстановления очков движения
    public void ResetMovePoints()
    {
        currentMovePoints = definition.baseMovePoints;
    }
}

[System.Serializable]
public class Missile
{
    public string missileType; // Тип ракеты (например, "CruiseMissile", "BallisticMissile")
    public int range; // Дальность пуска
    public int damage; // Урон, наносимый ракетой

    public Missile(string type, int range, int damage)
    {
        this.missileType = type;
        this.range = range;
        this.damage = damage;
    }
}