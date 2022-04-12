using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatCalc : MonoBehaviour
{
    [SerializeField] Stats unitStats;

    public int baseHP; //stats at level 1
    public int baseMP;
    public int baseStr;
    public int baseMag;
    public int baseDef;
    public int baseRes;
    public int baseAgi;

    public int levelHP; //how much of a stat is gained per level
    public int levelMP;
    public int levelStr;
    public int levelMag;
    public int levelDef;
    public int levelRes;
    public int levelAgi;

    int equipHP; //stat modifier of equipment
    int equipMP;
    int equipStr;
    int equipMag;
    int equipDef;
    int equipRes;
    int equipAgi;

    int buffStr; //stat buffs from skills, status effects, etc
    int buffMag;
    int buffDef;
    int buffRes;
    int buffAgi;

    void Start()
    {
        //unitStats = this.gameObject.GetComponent<Stats>(); 
        //FOR SOME FUCKING REASON THIS RANDOMLY STOPPED WORKING IDK WHY!!!!!!!!!!!!!!
    }


    public void CalculateStats()
    {
        int lvl = unitStats.level;

        int HP = baseHP + (levelHP * (lvl - 1)) + equipHP; //level -1 because level starts at 1
        int MP = baseMP + (levelMP * (lvl - 1)) + equipMP;
        int str = baseStr + (levelStr * (lvl - 1)) + equipStr + buffStr;
        int mag = baseMag + (levelMag * (lvl - 1)) + equipMag + buffMag;
        int def = baseDef + (levelDef * (lvl - 1)) + equipDef + buffDef;
        int res = baseRes + (levelRes * (lvl - 1)) + equipRes + buffRes;
        int agi = baseAgi + (levelAgi * (lvl - 1)) + equipAgi + buffAgi; //if equip/buffs are negative, will need to make sure it cant go below 0? maybe only for hp/mp

        unitStats.SetStats(HP, MP, str, mag, def, res, agi);

    }

    public void UpdateEquipment(int hp, int mp, int str, int mag, int def, int res, int agi)
    {
        //update values here to cut down on amount of public variables
        equipHP += hp;
        equipMP += mp;
        equipStr += str;
        equipMag += mag;
        equipDef += def;
        equipRes += res;
        equipAgi += agi;

        CalculateStats();

    }

    public void UpdateBuffs(int str, int mag, int def, int res, int agi)
    {
        buffStr += str; //+= so buffs dont override eachother, call this function with - values to remove buffs
        buffMag += mag;
        buffDef += def;
        buffRes += res;
        buffAgi += agi;

        CalculateStats();
    }

   
}
