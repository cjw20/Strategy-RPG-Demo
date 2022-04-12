using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName;
    public string value; //base cost of item for buying/selling
    public string description;
    public string ID; //for sorting, saving?
    public bool consumable;
    public bool equipable;
    public bool keyItem;
    public bool battleOnly; //consumable only during battle (potions)
    public int numberHeld;

    //icon

    public virtual void Use(GameObject target)
    {

    }

   
}
