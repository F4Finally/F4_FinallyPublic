using System.Collections;
using System.Collections.Generic;

using UnityEngine;


[System.Serializable]
public class SkillData
{
    public string skillName;
    public string description;
    public float effectValue;
    public Sprite skillIcon;
    public float coolDown;
    public float duration; //지속시간
    public GameObject effectPrefab;
    public SkillApply returnMySkillApply(Companion companion)
    {
        switch (skillName)
        {
            case "일반 공격": return new NormalAttack(this, companion);
            case "치명타 공격": return new CriticalAttack(this, companion);
            case "칼로 슬라임 베기 ": return new C001Skill(this, companion);
            case "젤리의 호흡": return new C002Skill(this, companion);
            case "비상식량": return new C003Skill(this, companion);
            case "젤리가 쓰러지지 않아!": return new C004Skill(this, companion);
            case "강력한 한방": return new R001Skill(this, companion);
            case "단단묵직": return new R002Skill(this, companion);
            case "구급상자": return new R003Skill(this, companion);
            case "난 둘다": return new R004Skill(this, companion);
            case "진통제": return new R005Skill(this, companion);
            case "폭주": return new R006Skill(this, companion);
            case "얼음!": return new R007Skill(this, companion);
            case "고무고무": return new E001Skill(this, companion);
            case "파이어브레스": return new E002Skill(this, companion);
            case "다재다능": return new E003Skill(this, companion);
            case "졸려": return new E004Skill(this, companion);







        }
        return null;
    }

}
[System.Serializable]
public abstract class SkillApply
{
    public Companion myCompanion;
    public SkillData myskilldata;
    public abstract void executeSkill();
    public float waitTime = 0;
    public abstract void checkExecute();

    public SkillApply(SkillData myskilldata, Companion myCompanion)
    {
        this.myskilldata = myskilldata;
        this.myCompanion = myCompanion;
    }

}

[System.Serializable]
public abstract class BuffSkillApply : SkillApply
{
    public BuffSkillApply(SkillData data, Companion myCompanion) : base(data, myCompanion) { }
    public enum TargetType { Self, SingleAlly, AllAllies, SingleEnemy, AllEnemies }

    public TargetType targetType;
    private GameObject spawnedEffect;
    protected bool nowBuffApplied;
    protected Coroutine nowBuffCoroutine;
    public override void checkExecute()
    {
        if (nowBuffApplied == true) return;
        waitTime += Time.deltaTime;
        if (waitTime > myskilldata.coolDown)
        {
            waitTime -= myskilldata.coolDown;
            executeSkill();
        }
    }
    public override void executeSkill()
    {
        if (nowBuffCoroutine != null) myCompanion.StopCoroutine(nowBuffCoroutine);
        nowBuffCoroutine = myCompanion.StartCoroutine(buffCoroutine());
    }
    IEnumerator buffCoroutine()
    {
        nowBuffApplied = true;
        StartBuff();
        yield return new WaitForSeconds(myskilldata.duration);
        EndBuff();
        nowBuffApplied = false;
    }
    protected void SpawnEffect()
    {
        if (myskilldata.effectPrefab != null)
        {
            spawnedEffect = Object.Instantiate(myskilldata.effectPrefab, myCompanion.transform.position, Quaternion.identity);
            spawnedEffect.transform.SetParent(myCompanion.transform);
        }
    }

    protected void DestroyEffect()
    {
        if (spawnedEffect != null)
        {
            Object.Destroy(spawnedEffect);
        }
    }
    public abstract void StartBuff();
    public abstract void EndBuff();


}

[System.Serializable]
public abstract class AttackSkillApply : SkillApply
{

    public AttackSkillApply(SkillData data, Companion myCompanion) : base(data, myCompanion) { }

    public void AttackEnemy(int maxtargetnum, float damage)
    {
        //Debug.Log($"{maxtargetnum}마리를 {damage}로 공격");
    }
    public override void checkExecute()
    {
        waitTime += Time.deltaTime;
        if (waitTime > myskilldata.coolDown)
        {
            waitTime -= myskilldata.coolDown;
            executeSkill();
        }
    }
}

// 공통 스킬 
public class NormalAttack : AttackSkillApply
{
    public NormalAttack(SkillData data, Companion myCompanion) : base(data, myCompanion) { }


    public float CalculateDamage(CompanionData data)
    {
        return data.GetCurrentStats().attack * myskilldata.effectValue;
    }

    public override void executeSkill()
    {
        //AttackEnemy(1, CalculateDamage(myCompanion.data));
    }

}

public class CriticalAttack : AttackSkillApply
{
    public CriticalAttack(SkillData data, Companion myCompanion) : base(data, myCompanion) { }


    public float CalculateDamage(CompanionData data)
    {
        return data.GetCurrentStats().attack * myskilldata.effectValue;
    }

    public override void executeSkill()
    {
        AttackEnemy(1, CalculateDamage(myCompanion.data));
    }


}


//개별 스킬들 
public class C001Skill : BuffSkillApply
{
    private float currentStat;
    public C001Skill(SkillData data, Companion myCompanion) : base(data, myCompanion) { }
    public override void StartBuff()
    {
        SpawnEffect();
        currentStat = myCompanion.data.GetCurrentStats().defense;
        float buffAmount = currentStat * myskilldata.effectValue;

        myCompanion.data.temporaryBuffs[StatType.Defense] = buffAmount;

    }
    public override void EndBuff()
    {

        myCompanion.data.temporaryBuffs.Remove(StatType.Defense);
        DestroyEffect();
    }

}
public class C002Skill : BuffSkillApply
{
    private float currentStat;
    public C002Skill(SkillData data, Companion myCompanion) : base(data, myCompanion) { }
    public override void StartBuff()
    {
        SpawnEffect();
        currentStat = myCompanion.data.GetCurrentStats().attack;
        float buffAmount = currentStat * myskilldata.effectValue;

        myCompanion.data.temporaryBuffs[StatType.Attack] = buffAmount;
    }
    public override void EndBuff()
    {
        myCompanion.data.temporaryBuffs.Remove(StatType.Attack);
        DestroyEffect();
    }


}
public class C003Skill : BuffSkillApply
{
    public C003Skill(SkillData data, Companion myCompanion) : base(data, myCompanion) { }
    public override void StartBuff()
    {
        SpawnEffect();
        float currentHealth = myCompanion.data.GetCurrentStats().hp;
        float maxHealth = myCompanion.data.dataSO.baseStats.hp;

        float healAmount = myskilldata.effectValue;
        float newHealth = Mathf.Min(currentHealth + healAmount, maxHealth);

        myCompanion.data.GetCurrentStats().hp = newHealth;
        float actualHealAmount = newHealth - currentHealth;

        Debug.Log($"체력이 {actualHealAmount} 회복되었습니다. 현재 체력: {newHealth}/{maxHealth}");
    }
    public override void EndBuff()
    {
        DestroyEffect();
    }


}

public class C004Skill : BuffSkillApply
{
    private bool isSkillActivated = false;
    private GameObject spawnedEffect = null;
    public C004Skill(SkillData data, Companion myCompanion) : base(data, myCompanion) { }

    public override void StartBuff()
    {
        myCompanion.StartCoroutine(MonitorHealthAndActivateSkill());
    }
    private IEnumerator MonitorHealthAndActivateSkill()
    {
        while (!isSkillActivated)
        {
            if (myCompanion.data.GetCurrentStats().hp <= 5 && !isSkillActivated)
            {
                ActivateSkill();
                break; // 스킬이 발동되면 코루틴을 종료합니다.
            }
            yield return null;
        }
    }
    private void ActivateSkill()
    {
        isSkillActivated = true;
        SpawnEffect();
        myCompanion.data.GetCurrentStats().hp = 1; // 체력을 1로 설정
        myCompanion.isInvulnerable = true; // 무적 상태 설정
        Debug.Log($"{myCompanion.dataSO.name}의 생존 스킬이 발동되었습니다! {myskilldata.duration}초 동안 무적 상태가 됩니다.");
        myCompanion.StartCoroutine(EndSkillAfterDuration());
    }
    private IEnumerator EndSkillAfterDuration()
    {
        yield return new WaitForSeconds(myskilldata.duration);
        EndBuff();
    }
    private void SpawnEffect()
    {
        if (myskilldata.effectPrefab != null && spawnedEffect == null)
        {
            spawnedEffect = Object.Instantiate(myskilldata.effectPrefab, myCompanion.transform.position, Quaternion.identity);
            spawnedEffect.transform.SetParent(myCompanion.transform);
        }
    }
    public override void EndBuff()
    {
        if (isSkillActivated)
        {
            myCompanion.isInvulnerable = false; // 무적 상태 해제
            if (spawnedEffect != null)
            {
                Object.Destroy(spawnedEffect);
                spawnedEffect = null;
            }
            Debug.Log($"{myCompanion.dataSO.name}의 생존 스킬 효과가 종료되었습니다.");
        }
    }
}
public class R001Skill : BuffSkillApply
{
    private float currentStat;
    public R001Skill(SkillData data, Companion myCompanion) : base(data, myCompanion) { }
   
    public override void StartBuff()
    {
        SpawnEffect();
        currentStat = myCompanion.data.GetCurrentStats().attack;
        float buffAmount = currentStat * myskilldata.effectValue;

        myCompanion.data.temporaryBuffs[StatType.Attack] = buffAmount;
    }
    public override void EndBuff()
    {
        myCompanion.data.temporaryBuffs.Remove(StatType.Attack);
        DestroyEffect();
    }

}
public class R002Skill : BuffSkillApply
{
    private float currentStat;
    public R002Skill(SkillData data, Companion myCompanion) : base(data, myCompanion) { }
    public override void StartBuff()
    {
        SpawnEffect();
        currentStat = myCompanion.data.GetCurrentStats().defense;
        float buffAmount = currentStat * myskilldata.effectValue;

        myCompanion.data.temporaryBuffs[StatType.Defense] = buffAmount;
    }
    public override void EndBuff()
    {
        myCompanion.data.temporaryBuffs.Remove(StatType.Defense);
        DestroyEffect();
    }

}
public class R003Skill : BuffSkillApply
{
    public R003Skill(SkillData data, Companion myCompanion) : base(data, myCompanion) { }
    public override void StartBuff()
    {
        SpawnEffect();
        float currentHealth = myCompanion.data.GetCurrentStats().hp;
        float maxHealth = myCompanion.data.dataSO.baseStats.hp;

        float healAmount = myskilldata.effectValue;
        float newHealth = Mathf.Min(currentHealth + healAmount, maxHealth);

        myCompanion.data.GetCurrentStats().hp = newHealth;
        float actualHealAmount = newHealth - currentHealth;

        Debug.Log($"체력이 {actualHealAmount} 회복되었습니다. 현재 체력: {newHealth}/{maxHealth}");
    }
    public override void EndBuff()
    {
        DestroyEffect();
    }


}

public class R004Skill : BuffSkillApply
{
    private float currentStat1;
    private float currentStat2;
    public R004Skill(SkillData data, Companion myCompanion) : base(data, myCompanion) { }
    public override void StartBuff()
    {
        SpawnEffect();
        currentStat1 = myCompanion.data.GetCurrentStats().defense;
        currentStat2 = myCompanion.data.GetCurrentStats().attack;
        float buffAmount1 = currentStat1 * myskilldata.effectValue;
        float buffAmount2 = currentStat2 * myskilldata.effectValue;


        myCompanion.data.temporaryBuffs[StatType.Defense] = buffAmount1;
        myCompanion.data.temporaryBuffs[StatType.Attack] = buffAmount2;

    }

    
    public override void EndBuff() 
    {
        myCompanion.data.temporaryBuffs.Remove(StatType.Defense);
        myCompanion.data.temporaryBuffs.Remove(StatType.Attack);
        DestroyEffect();
    }


}
public class R005Skill : BuffSkillApply
{
  
    public R005Skill(SkillData data, Companion myCompanion) : base(data, myCompanion) { }
    public override void StartBuff()
    {
        SpawnEffect();
        float currentHealth = myCompanion.data.GetCurrentStats().hp;
        float maxHealth = myCompanion.data.dataSO.baseStats.hp;

        float healAmount = myskilldata.effectValue;
        float newHealth = Mathf.Min(currentHealth + healAmount, maxHealth);

        myCompanion.data.GetCurrentStats().hp = newHealth;
        float actualHealAmount = newHealth - currentHealth;

        Debug.Log($"체력이 {actualHealAmount} 회복되었습니다. 현재 체력: {newHealth}/{maxHealth}");
    }
    public override void EndBuff()
    {
        DestroyEffect();
    }

}
public class R006Skill : BuffSkillApply
{
    private float currentStat;
    public R006Skill(SkillData data, Companion myCompanion) : base(data, myCompanion) { }
    public override void StartBuff()
    {
        SpawnEffect();
        currentStat = myCompanion.data.GetCurrentStats().attack;
        float buffAmount = currentStat * myskilldata.effectValue;

        myCompanion.data.temporaryBuffs[StatType.Attack] = buffAmount;
    }
    public override void EndBuff()
    {
        myCompanion.data.temporaryBuffs.Remove(StatType.Attack);
        DestroyEffect();
    }


}
public class R007Skill : BuffSkillApply
{
    private float currentStat;
    public R007Skill(SkillData data, Companion myCompanion) : base(data, myCompanion) { }
    public override void StartBuff()
    {
        SpawnEffect();
        currentStat = myCompanion.data.GetCurrentStats().defense;
        float buffAmount = currentStat * myskilldata.effectValue;

        myCompanion.data.temporaryBuffs[StatType.Defense] = buffAmount;
    }
    public override void EndBuff()
    {
        myCompanion.data.temporaryBuffs.Remove(StatType.Defense);
        DestroyEffect();
    }


}

public class E001Skill : BuffSkillApply
{
    private float originalAttackRange;
    public E001Skill(SkillData data, Companion myCompanion) : base(data, myCompanion) { }
    public override void StartBuff()
    {
        SpawnEffect();
        originalAttackRange = myCompanion.data.dataSO.attackRange;
        float buffAmount = originalAttackRange * myskilldata.effectValue;

        myCompanion.data.dataSO.attackRange += buffAmount;
        
    }

    public override void EndBuff()
    {
        myCompanion.data.dataSO.attackRange = originalAttackRange;
        DestroyEffect();
        
    }


}
public class E002Skill : AttackSkillApply
{
    public E002Skill(SkillData data, Companion myCompanion) : base(data, myCompanion) { }
    public override void executeSkill()
    {
        List<Enemy> targets = FindMultipleEnemies();
        float damage = CalculateDamage();

        foreach (Enemy enemy in targets)
        {
            enemy.TakeDamage((System.Numerics.BigInteger)damage);
            Debug.Log($"{myCompanion.dataSO.name}이(가) {enemy.name}에게 {damage} 데미지를 입혔습니다.");
        }

        // 이펙트 생성
        if (myskilldata.effectPrefab != null)
        {
            GameObject effect = Object.Instantiate(myskilldata.effectPrefab, myCompanion.transform.position, Quaternion.identity);
            Object.Destroy(effect, myskilldata.duration);
        }
    }

    private List<Enemy> FindMultipleEnemies()
    {
        List<Enemy> targets = new List<Enemy>();
        float companionX = myCompanion.transform.position.x;
        float maxX = companionX + myCompanion.dataSO.attackRange;

        foreach (Enemy enemy in MainScene.Instance.returnNowActiveEnemies())
        {
            if (enemy == null) continue;
            float enemyX = enemy.transform.position.x;
            if (enemyX <= maxX)
            {
                targets.Add(enemy);
                if (targets.Count >= myskilldata.effectValue) // effectValue를 최대 타겟 수로 사용
                {
                    break;
                }
            }
        }

        return targets;
    }

    private float CalculateDamage()
    {
        return myCompanion.data.GetCurrentStats().attack * myskilldata.effectValue;
    }


}
public class E003Skill : BuffSkillApply
{
    private float currentStat1;
    private float currentStat2;
    private float currentStat3;
    public E003Skill(SkillData data, Companion myCompanion) : base(data, myCompanion) { }
    public override void StartBuff()
    {
        SpawnEffect();
        currentStat1 = myCompanion.data.GetCurrentStats().defense;
        currentStat2 = myCompanion.data.GetCurrentStats().attack;
        currentStat3 = myCompanion.data.GetCurrentStats().hp;
        float buffAmount1 = currentStat1 * myskilldata.effectValue;
        float buffAmount2 = currentStat2 * myskilldata.effectValue;
        float buffAmount3 = currentStat3 * myskilldata.effectValue;

        myCompanion.data.temporaryBuffs[StatType.Defense] = buffAmount1;
        myCompanion.data.temporaryBuffs[StatType.Attack] = buffAmount2;
        myCompanion.data.temporaryBuffs[StatType.Health] = buffAmount3;
    }


    public override void EndBuff()
    {
        myCompanion.data.temporaryBuffs.Remove(StatType.Defense);
        myCompanion.data.temporaryBuffs.Remove(StatType.Attack);
        myCompanion.data.temporaryBuffs.Remove(StatType.Health);
        DestroyEffect();
    }


}
public class E004Skill : BuffSkillApply
{
    public E004Skill(SkillData data, Companion myCompanion) : base(data, myCompanion) { }

    public override void StartBuff()
    {
        SpawnEffect();
        myCompanion.isInvulnerable = true;
    }
    public override void EndBuff()
    {
        myCompanion.isInvulnerable = false;
        DestroyEffect();
    }
}