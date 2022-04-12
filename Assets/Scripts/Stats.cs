using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DamageType { PHYSICAL, MAGICAL, TRUE, HEAL} //physical resisted by defense, magical by res, true cant be resisted
public class Stats : MonoBehaviour
{
    //contains combat relevant stats and functions
    StatCalc statCalc;
    public int level;
    public int currentXP; //maybe doesnt need to be public 
    public WeaponType weaponType; //type of weapon this unit can equip

    public Equipment equipedWeapon;
    public Equipment equipedArmor;
    public Equipment equipedAccessory;
    
    public int maxHP;
    public int currentHP;
    public int maxMP;
    public int currentMP; //used for skills

    public int str; //maybe use inheritance for each stat, have functions included for buffs, equipment, leveling etc
    public int def;
    public int mag;
    public int res;
    public int agi;
    //maybe move here instead of in unit

    public int minAttackRange; //this should be value of highest range of abilities for enemy script
    public int maxAttackRange;
   
    public Ability basicAttack; //ability that is used as basic attack
    public List<Ability> abilities;

    [SerializeField] Slider hpSlider;

    public int killXP; //amount of xp rewarded for killing unit. Damage w.out kill gives a %
    //drop $ items
    
    void Start()
    {
        statCalc = this.gameObject.GetComponent<StatCalc>(); //make sure this script if first in runtime order
        SetEquipment();
        statCalc.CalculateStats();
        currentHP = maxHP;
        currentMP = maxMP;
        hpSlider.maxValue = maxHP;
        hpSlider.value = currentHP;
        GetAttackRange();
    }

    public void GetAttackRange()
    {
        minAttackRange = basicAttack.minRange;
        maxAttackRange = basicAttack.maxRange;

        foreach(Ability a in abilities)
        {
            if(a.mpCost <= currentMP && a.unlockLevel <= level)
            {
                if (a.minRange < minAttackRange)
                    minAttackRange = a.minRange;
                if (a.maxRange > maxAttackRange)
                    maxAttackRange = a.maxRange;
            }
        }

        
    }

    void SetEquipment()
    {

        //sets each equiped item as equiped 
        if(equipedWeapon != null)
        {
            equipedWeapon.Equip(this.gameObject);
        }
        if (equipedArmor != null)
        {
            equipedArmor.Equip(this.gameObject);
        }
        if (equipedAccessory != null)
        {
            equipedAccessory.Equip(this.gameObject);
        }
    }

    public void GainXP(int amount)
    {
        currentXP += amount;
        while(currentXP >= 100 * (level + 1))
        {
            currentXP -= (100 * (level + 1)); //costs 100 times the next level to level up (level 2 takes 200, 3 takes 300 etc)
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        //display notification or something

        statCalc.CalculateStats();
    }

    public void SetStats(int hp, int mp, int strength, int magic, int defense, int resistance, int agility)
    {
        maxHP = hp;
        hpSlider.maxValue = maxHP; //heal to full?
        maxMP = mp;
        //slider

        str = strength;
        mag = magic;
        def = defense;
        res = resistance;
        agi = agility;
    }    

    public int TakeDamage(int power, DamageType type)
    {

        //later add things like hit chance or criticals
        int damageTaken = 0;

        switch (type)
        {
            case DamageType.PHYSICAL:
            {
                damageTaken = power - def;
                break;
            }
            case DamageType.MAGICAL:
            {
                damageTaken = power - res;
                break;
            }
            case DamageType.TRUE:
            {
                damageTaken = power;
                break;
            }
            case DamageType.HEAL:
            {
                damageTaken = -power; //- so it heals
                break;

            }
        }
        SetHP(-damageTaken);
        return damageTaken;
    }

    void SetHP(int change)
    {
        //returns 1 if unit dies for purpose of rewarding xp
        currentHP += change;
        if (currentHP >= maxHP)
        {
            currentHP = maxHP;
        }
        hpSlider.value = currentHP;

        if (currentHP <= 0)
        {
            this.gameObject.GetComponent<Unit>().currentTile.Unoccupy();

            if(BattleSystem.Instance.gameState == GameState.ALLYTURN)
            {
                AllyController.Instance.selectedUnit.GetComponent<Stats>().GainXP(killXP);
                //gives xp to unit who just acted. will need to change if it becomes possible to damage enemies outside of a units turn
            }
            if(this.gameObject.GetComponent<Unit>().faction == Faction.ENEMY)
            {
                //give exp
                EnemyManager.Instance.KilledEnemy(this.gameObject);
            }
            else
            {
                AllyController.Instance.KilledAlly(this.gameObject);
            }
            
        }
        if (BattleSystem.Instance.gameState == GameState.ALLYTURN)
            AllyController.Instance.selectedUnit.GetComponent<Stats>().GainXP(killXP / 10);
    }


    public void SetMP(int change)
    {
        currentMP += change;
        if (currentMP >= maxMP)
        {
            currentMP = maxMP;
        }

        if(currentMP < 0)
        {
            currentMP = 0;
            Debug.Log("MP cost check ignored?"); //this shouldnt ever happen?
        }
    }

}
