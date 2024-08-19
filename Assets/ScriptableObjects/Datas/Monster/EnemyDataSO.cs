using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public enum AttackType
{
    Melee,
    Ranged
}

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Enemy Data")]
public class EnemyDataSO : ScriptableObject
{
    public AttackType attackType;
    public BigInteger maxHealth = 5;
    public float moveSpeed = 5f;
    public BigInteger attackDamage = 5;
    public float attackSpeed;
    public float attackRange;
    public int stageLevel;
}
