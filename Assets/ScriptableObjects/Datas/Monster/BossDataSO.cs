using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Enemy Data", menuName = "Boss Enemy Data")]
public class BossDataSO : ScriptableObject
{
    public float health;
    public float attackDamage;
    public float moveSpeed;
}
