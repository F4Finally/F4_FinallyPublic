using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public class PlayerStateMachine : StateMachine
{
    public Player player { get; }
    public EnemySpawn enemySpawn { get; private set; }  
    public PlayerIdleState playerIdleState { get; private set; }
    public PlayerRunState playerRunState { get; private set; }
    public PlayerBaseAttackState playerBaseAttackState { get; private set; }

    public PlayerUltimateAttackState playerUltimateAttackState { get; private set; }    
    public PlayerHitState playerHitState { get; private set; }
    

    public  PlayerStateMachine(Player player)
    {
        this.player = player;
        playerIdleState = new PlayerIdleState(this);
        playerRunState = new PlayerRunState(this);
        playerBaseAttackState = new PlayerBaseAttackState(this);
        playerUltimateAttackState = new PlayerUltimateAttackState(this);
        playerHitState = new PlayerHitState(this);

    }
}