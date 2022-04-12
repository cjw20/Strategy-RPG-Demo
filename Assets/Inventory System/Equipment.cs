using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipType { WEAPON, ARMOR, ACCESSORY}
public enum WeaponType { SWORD, SPEAR, AXE, BOW, STAFF, OTHER, NOTWEAPON}
public class Equipment : Item
{
    public EquipType equipType;
    public WeaponType weaponType;
    public int equipHP; //stat modifier of equipment
    public int equipMP;
    public int equipStr;
    public int equipMag;
    public int equipDef;
    public int equipRes;
    public int equipAgi;

    //passives list

    public int numberEquiped; //subtract from number held to see if their are any left to equip

    public virtual void Equip(GameObject character)
    {
        numberEquiped++;
        character.GetComponent<StatCalc>().UpdateEquipment(equipHP, equipMP, equipStr, equipMag, equipDef, equipRes, equipRes);
    }

    public virtual void UnEquip(GameObject character)
    {
        numberEquiped--;
        character.GetComponent<StatCalc>().UpdateEquipment(-equipHP, -equipMP, -equipStr, -equipMag, -equipDef, -equipRes, -equipRes);
    }
}
