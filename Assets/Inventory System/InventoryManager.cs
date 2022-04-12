using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private static InventoryManager instance;
    public static InventoryManager Instance { get { return instance; } }  

    Dictionary<Item, int> heldItems;

    //might need to make equipment separate dictionary
    
    void Awake()
    {
        instance = this;
    }

    public void AddItem(Item item, int amount)
    {
        if (heldItems.ContainsKey(item))
        {
            heldItems[item] += amount;
            
            //consider max amount?
        }

        else
        {
            heldItems.Add(item, amount);
        }
        
    }

    public void RemoveItem(Item item, int amount)
    {
        heldItems[item] -= amount;
        

        if(heldItems[item] <= 0)
        {
            heldItems.Remove(item);
        }
    }


    public bool CheckInventory(Item item)
    {
        bool inInventory = false;
        if (heldItems.ContainsKey(item))
        {
            inInventory = true;
        }
        return inInventory;
    }

    public int CheckAmount(Item item)
    {
        //for displaying amount in ui
        int amount = heldItems[item];

        return amount;
    }

    public List<Item> GetConsumables()
    {
        List<Item> heldConsumables = new List<Item>();
        foreach(KeyValuePair<Item, int> entry in heldItems)
        {
            if (entry.Key.consumable)
            {
                heldConsumables.Add(entry.Key);
            }            
        }

        return heldConsumables;
    }

    public List<Item> GetEquipment()
    {
        List<Item> heldEquipment = new List<Item>();
        foreach (KeyValuePair<Item, int> entry in heldItems)
        {
            if (entry.Key.equipable)
            {
                heldEquipment.Add(entry.Key);
            }
        }

        return heldEquipment;
    }

    public List<Item> GetKeyItems()
    {
        List<Item> heldKeys = new List<Item>();
        foreach (KeyValuePair<Item, int> entry in heldItems)
        {
            if (entry.Key.keyItem)
            {
                heldKeys.Add(entry.Key);
            }
        }

        return heldKeys;
    }


}
