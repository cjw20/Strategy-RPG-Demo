using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public string abilityName;
    public string description; //what the ability does
    public int unlockLevel;
    public List<AbilityBehaviors> behaviors; //lift of effects
    public bool canTargetAlly; //true if ability can target allies (healing, buffs)

    //public bool healing; //heals instead of doing damage
    public int minRange;
    public int maxRange;
    public DamageType type;
    //status effect
    //base hit/crit rates
    public int power; //base power of ability w/out modifers
    public int mpCost;
    public bool isAOE; //true when skill is an aoe attack
    public List<Vector2> area; //tiles in aoe may need to change data type 
    public bool support; //non attack abilities, heal buffs
    public GameObject effect; //visual and sound effect
    


    public virtual bool DisplayRange(GameObject user)
    {        
        if(mpCost > user.GetComponent<Stats>().currentMP)
        {
            return false; //might change how this works if button is greyed out
        }
        if (support)
        {
            GridManager.Instance.DisplaySupportRange(user.GetComponent<Unit>().currentTile, minRange, maxRange);
        }
        else
        {
            GridManager.Instance.DisplayAttackRange(user.GetComponent<Unit>().currentTile, minRange, maxRange);
        }
        

        return true; //true if ability is useable (has sufficent mp)
    }

    public virtual void DisplayForecast(GameObject user)
    {

    }
    public virtual void UseAbility(GameObject user, GameObject target)
    {
        int attackPower = CalculatePower(user);
        target.GetComponent<Stats>().TakeDamage(attackPower, type);
        if(effect != null)
        {
            GameObject e = Instantiate(effect);
            e.transform.position = target.transform.position;
        }
        user.GetComponent<Stats>().SetMP(-mpCost); //subtracts used mp from current
    }
    public virtual int CalculatePower(GameObject user)
    {
        int attackPower = 0;
        Stats userStats = user.GetComponent<Stats>();
        switch (type)
        {
            case DamageType.PHYSICAL:
                {
                    attackPower = power + userStats.str;
                    break;
                }
            case DamageType.HEAL:
                {
                    attackPower = power + userStats.mag; //might use res or something else
                    break;
                }
            case DamageType.MAGICAL:
                {
                    attackPower = power + userStats.mag;
                    break;
                }
            case DamageType.TRUE:
                {
                    attackPower = power;
                    break;
                }
        }

        return attackPower;
        
    }

    public virtual int CalculateResult(int power, GameObject target)
    {
        Stats targetStats = target.GetComponent<Stats>();
        int result = 0;
        switch (type)
        {
            case DamageType.PHYSICAL:
                {
                    result = power - targetStats.def;

                    break;
                }
            case DamageType.HEAL:
                {
                    result = power; //might use res or something else
                    break;
                }
            case DamageType.MAGICAL:
                {
                    result = power - targetStats.res;
                    break;
                }
            case DamageType.TRUE:
                {
                    result = power;
                    break;
                }
        }

        if(result < 0)
        {
            result = 0;
        }
        return result;
    }

    

}
