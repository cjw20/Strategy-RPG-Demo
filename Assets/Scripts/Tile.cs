using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int xCord;
    public int yCord;
    public bool allyOccupied; //so allies can pass through each other
    public bool enemyOccupied;
    public GameObject unitInTile; //may change type later (stats)

    public float moveCost;
    //enum terrain type like water, unpassable, plain

    public List<Tile> neighbours;

    public GameObject highlight;
    public GameObject moveHighlight;
    public GameObject attackHighlight;
    public GameObject supportHighlight;
    public bool inRange;
    
    List<Tile> highlightedTiles; //tiles highlighted when moused over enemy

    void Start()
    {
        xCord = (int) transform.position.x;
        yCord = (int) transform.position.y;

        GridManager.Instance.AddTile(xCord, yCord, this);
        neighbours = new List<Tile>();
    }

    public void Occupy(Faction faction, GameObject character)
    {
        if (faction == Faction.ALLY)
        {
            allyOccupied = true;
        }
        if(faction == Faction.ENEMY)
        {
            enemyOccupied = true;
        }

        unitInTile = character;
    }


    public void Unoccupy()
    {
        allyOccupied = false;
        enemyOccupied = false;
        unitInTile = null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(unitInTile != null && !BattleUIHandler.Instance.inUI && BattleSystem.Instance.gameState == GameState.ALLYTURN && highlight.activeInHierarchy)
            {
                BattleUIHandler.Instance.ShowStatWindow(unitInTile);
            }
        }
            
    }
    private void OnMouseDown()
    {
        if (BattleUIHandler.Instance.inUI)
        {
            return; //cant click on tiles when in ui
        }
        
        if(BattleSystem.Instance.gameState == GameState.ALLYTURN)
        {
            
            AllyController.Instance.SelectTile(this);
        }
        GridManager.Instance.selectedTile = this; //may not need?    
        
    }
    public float GetMoveCost()
    {
        float cost = moveCost;
        if (BattleSystem.Instance.gameState == GameState.ALLYTURN)
        {
            if (enemyOccupied)
            {
                cost = Mathf.Infinity; //pathfinding will treat this tile as untraversalble
            } 
        }

        if (BattleSystem.Instance.gameState == GameState.ENEMYTURN)
        {
            if (allyOccupied)
            {
                cost = Mathf.Infinity;
            }
        }

        return cost;
    }

    public float GetEnemyMoveCost()
    {
        //movecost that always acts like its enemy turn
        float cost = moveCost;

        if (allyOccupied)
        {
            cost = Mathf.Infinity;
        }
        return cost;
    }
    private void OnMouseEnter()
    {
        if (!BattleUIHandler.Instance.inUI && BattleSystem.Instance.gameState == GameState.ALLYTURN)
        {
            highlight.SetActive(true);

            if ((enemyOccupied || allyOccupied))
            {
                BattleUIHandler.Instance.ShowHoverWindow(unitInTile);
            }

            if (enemyOccupied && AllyController.Instance.state == ALLYTURNSTATE.IDLE)
            {
                Stats enemyStats = unitInTile.GetComponent<Stats>();
                enemyStats.GetAttackRange();
                highlightedTiles = Pathfinding.Instance.GetEnemyRange(this, unitInTile.GetComponent<Unit>().moveDistance, enemyStats.minAttackRange, enemyStats.maxAttackRange);

                foreach(Tile t in highlightedTiles)
                {
                    t.attackHighlight.SetActive(true);
                }
                //show move and attack range???????
            }
        }
        
    }

    private void OnMouseExit()
    {
        highlight.SetActive(false);
        if ((enemyOccupied || allyOccupied) && BattleSystem.Instance.gameState == GameState.ALLYTURN)
        {
            BattleUIHandler.Instance.HideHoverWindow(); //probably should only call this when active

            if(highlightedTiles != null && AllyController.Instance.state == ALLYTURNSTATE.IDLE)
            {
                foreach (Tile t in highlightedTiles)
                {
                    t.attackHighlight.SetActive(false);
                }
            }
           
        }

        
    }
}
