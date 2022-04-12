using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private static GridManager instance;
    public static GridManager Instance { get { return instance; } }

    [SerializeField] int width, height;
    public Tile[,] mapTiles;
    
    public Tile selectedTile;
    List<Tile> highlightedTiles;
    
    void Awake()
    {
        instance = this;
        //may need to make check for other instances depending on how scenes are loaded

        mapTiles = new Tile[width, height];

    }

   
    public void AddTile(int xPos, int yPos, Tile tile)
    {
        mapTiles[xPos, yPos] = tile;
       
    }

    public Tile GetTile(int xPos, int yPos)
    {
        return mapTiles[xPos, yPos];
    }


   
    public void GetNeighbours()
    {

        foreach(Tile t in mapTiles)
        {
            if (t.xCord > 0)
                mapTiles[t.xCord, t.yCord].neighbours.Add(mapTiles[t.xCord - 1, t.yCord]);
            if (t.xCord < width - 1)
                mapTiles[t.xCord, t.yCord].neighbours.Add(mapTiles[t.xCord + 1, t.yCord]);
            if (t.yCord > 0)
                mapTiles[t.xCord, t.yCord].neighbours.Add(mapTiles[t.xCord, t.yCord - 1]);
            if (t.yCord < height - 1)
                mapTiles[t.xCord, t.yCord].neighbours.Add(mapTiles[t.xCord, t.yCord + 1]);
        }
        
    }


    public void DisplayAttackRange(Tile origin, int minRange, int maxRange)
    {
        highlightedTiles = new List<Tile>();
        foreach(Tile t in mapTiles)
        {
            int dist = CheckDistance(origin, t);

            if(dist <= maxRange && dist >= minRange)
            {
                highlightedTiles.Add(t);
            }
        }
        
        foreach(Tile t in highlightedTiles)
        {
            t.attackHighlight.SetActive(true);
            t.inRange = true;
        }
    }
    public void DisplaySupportRange(Tile origin, int minRange, int maxRange)
    {
        highlightedTiles = new List<Tile>();
        foreach (Tile t in mapTiles)
        {
            int dist = CheckDistance(origin, t);

            if (dist <= maxRange && dist >= minRange)
            {
                highlightedTiles.Add(t);
            }
        }

        foreach (Tile t in highlightedTiles)
        {
            t.supportHighlight.SetActive(true);
            t.inRange = true;
        }
    }

    public void HideAttackRange()
    {
        if(highlightedTiles == null)
        {
            return;
        }
        foreach(Tile t in highlightedTiles)
        {
            t.attackHighlight.SetActive(false);
            t.supportHighlight.SetActive(false);
            t.inRange = false;
        }
    }

    public int CheckDistance(Tile origin, Tile other)
    {
        int xDif = Mathf.Abs(other.xCord - origin.xCord);
        int yDif = Mathf.Abs(other.yCord - origin.yCord);

        return yDif + xDif;
    }


    
}
