using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Faction { ALLY, ENEMY} //may move this declaration elsewere
public class Unit : MonoBehaviour
{
    //unit handles information acessable from map for both allies and enemies

    public string unitName;
    public Faction faction;

    public int xCord;
    public int yCord;
    public Tile currentTile;
    Tile lastTile;


    public bool hasAction; //unit hasn't used turn
    public bool hasMove;


    public float moveDistance; //how far the unit can move on their turn
    //public int attackRange;
    List<Tile> movePath;

    
    void Start()
    {
        UpdatePosition();
        hasAction = true;
        hasMove = true;
    }

    public void UpdatePosition()
    {
        xCord = (int)transform.position.x;
        yCord = (int)transform.position.y;


        if(currentTile != null) //doesn't do this on first call
        {
            lastTile = currentTile; //stores original position
            lastTile.Unoccupy(); //empties old tile
        }

        currentTile = GridManager.Instance.GetTile(xCord, yCord);
        currentTile.Occupy(faction, this.gameObject); //tells tile who is on it



    }


   
}
    
