using UnityEngine;
[System.Serializable]
public class Stats
{
    public float combatPower;
    public float hp;
    public float attack;
    public float defense;

    public Stats(float hp = 0, float attack = 0, float defense = 0)
    {
        this.hp = hp;
        this.attack = attack;
        this.defense = defense;
        CalculateCombatPower();
    }

    public Stats(Stats other)
    {
        this.hp = other.hp;
        this.attack = other.attack;
        this.defense = other.defense;
        this.combatPower = other.combatPower;
    }

    private void CalculateCombatPower()
    {
        combatPower = hp + attack + defense; 
    }

    public Stats Clone()
    {
        return new Stats(this);
    }

    public void IncreaseStats(Stats increase)
    {
        hp += increase.hp;
        attack += increase.attack;
        defense += increase.defense;
        CalculateCombatPower();
    }

    public void DecreaseStats(Stats decrease)
    {
        hp = Mathf.Max(hp - decrease.hp, 0);
        attack = Mathf.Max(attack - decrease.attack, 0);
        defense = Mathf.Max(defense - decrease.defense, 0);
        CalculateCombatPower();
    }

    public void MultiplyStats(float multiplier)
    {
        hp *= multiplier;
        attack *= multiplier;
        defense *= multiplier;
        CalculateCombatPower();
    }

    public override string ToString()
    {
        return $"CombatPower: {combatPower}, HP: {hp}, Attack: {attack}, Defense: {defense}";
    }
}
