using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pathfinding : MonoBehaviour
{
    //move unit x from a to b along path

    private static Pathfinding instance;
    public static Pathfinding Instance { get { return instance; } }

    List<Tile> displayedRange;
    public bool closetOccupied; //true when returned closest is occupied

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        GridManager.Instance.GetNeighbours(); 
    }
    public List<Tile> Pathfind(Tile start, Tile target)
    {
        
        //pathfinding using Dijkstra's algorithm
        Dictionary<Tile, float> distance = new Dictionary<Tile, float>();
        Dictionary<Tile, Tile> previous = new Dictionary<Tile, Tile>();

        
        List<Tile> unvisited = new List<Tile>(); //nodes that haven't been checked yet
        List<Tile> currentPath = new List<Tile>(); //path that unit will follow

        distance[start] = 0; //starting point 
        previous[start] = null; //end of line

        foreach(Tile v in GridManager.Instance.mapTiles)
        {
            if(v != start)
            {
                distance[v] = Mathf.Infinity;
                previous[v] = null;  //initializes both dictionaries
            }

            unvisited.Add(v);

        }

        while(unvisited.Count > 0)
        {
            //might want to make this faster later if it takes too long
            

            Tile u = null;
            foreach(Tile possibleU in unvisited)
            {
                if(u == null || distance[possibleU] < distance[u])
                {
                    u = possibleU;
                }
            }

            if (u == target)
            {
               break; //may not want this
            }
                
            unvisited.Remove(u);

            foreach(Tile v in u.neighbours)
            {
                float alt = distance[u] + v.GetMoveCost();
                if(alt < distance[v])
                {
                    distance[v] = alt;
                    previous[v] = u;
                }
            }
        }
        if(distance[target] == Mathf.Infinity)
        {
            List<Tile> onlyOrigin = new List<Tile>();
            onlyOrigin.Add(start);
            return onlyOrigin; //returns only origin when distance to all other tiles is infinity
        }
        
        //if(previous[target] == null)
        {
            //no route!
            //can check for move distance here?
           // return(null);
        }

        Tile curr = target;

        while(curr != null)
        {
            currentPath.Add(curr);
            curr = previous[curr];
        }

        currentPath.Reverse();
        
        return (currentPath);
    }


    public List<Tile> DisplayMoveRange(Tile origin, float range)
    {
        //displays range of tiles that can be moved to
        List<Tile> tilesInRange = new List<Tile>();
        int r = (int)range;

        int xPos = origin.xCord;
        int yPos = origin.yCord;

        
        foreach (Tile t in GridManager.Instance.mapTiles)
        {

            List<Tile> distancePath = Pathfind(origin, t);
            distancePath.Remove(origin);
            float moveCost = 0;
            foreach(Tile j in distancePath)
            {
                moveCost += j.GetMoveCost();
                
            } //this part of foreach loop can be own function to find reachable tiles

            if (moveCost <= r && moveCost != 0)
            {
                tilesInRange.Add(t);
            }
        }
        foreach (Tile t in tilesInRange)
        {
            t.moveHighlight.SetActive(true);

            if(!t.allyOccupied && !t.enemyOccupied) //cant move onto occupied spaces
            {
                t.inRange = true;
            }
            
        }
        displayedRange = tilesInRange;
        return tilesInRange;
    }

    List<Tile> EnemyPathfind(Tile start, Tile target)
    {
        //For use when checking enemy range on player turn

        //pathfinding using Dijkstra's algorithm
        Dictionary<Tile, float> distance = new Dictionary<Tile, float>();
        Dictionary<Tile, Tile> previous = new Dictionary<Tile, Tile>();


        List<Tile> unvisited = new List<Tile>(); //nodes that haven't been checked yet
        List<Tile> currentPath = new List<Tile>(); //path that unit will follow

        distance[start] = 0; //starting point 
        previous[start] = null; //end of line

        foreach (Tile v in GridManager.Instance.mapTiles)
        {
            if (v != start)
            {
                distance[v] = Mathf.Infinity;
                previous[v] = null;  //initializes both dictionaries
            }

            unvisited.Add(v);

        }

        while (unvisited.Count > 0)
        {
            //might want to make this faster later if it takes too long


            Tile u = null;
            foreach (Tile possibleU in unvisited)
            {
                if (u == null || distance[possibleU] < distance[u])
                {
                    u = possibleU;
                }
            }

            if (u == target)
            {
                break; //may not want this
            }

            unvisited.Remove(u);

            foreach (Tile v in u.neighbours)
            {
                float alt = distance[u] + v.GetEnemyMoveCost();
                if (alt < distance[v])
                {
                    distance[v] = alt;
                    previous[v] = u;
                }
            }
        }
        if (distance[target] == Mathf.Infinity)
        {
            List<Tile> onlyOrigin = new List<Tile>();
            onlyOrigin.Add(start);
            //return onlyOrigin; //returns only origin when distance to all other tiles is infinity
        }        

        Tile curr = target;

        while (curr != null)
        {
            currentPath.Add(curr);
            curr = previous[curr];
        }

        currentPath.Reverse();

        return (currentPath);
    }
    public void HideMoveRange()
    {
        if(displayedRange == null)
        {
            return;
        }
        //dont call unless movement range has been displayed or it freaks out
        foreach(Tile t in displayedRange)
        {
            t.moveHighlight.SetActive(false);
            t.inRange = false;
        }
    }


    public Tile GetClosestNeighbour(Tile target, Tile origin)
    {

        //finds the neighbour tile closest to origin

        float currentBest = Mathf.Infinity;
        Tile chosenTile = null;
        foreach(Tile t in target.neighbours)
        {
            float dist = 0;
            List<Tile> path = Pathfind(origin, t);
            foreach(Tile j in path)
            {
                dist += j.GetMoveCost();

            }
            
            if (dist < currentBest && !(t.enemyOccupied || t.allyOccupied))
            {
                currentBest = dist;
                chosenTile = t;
            }
        }

        return chosenTile;
    }

    public List<Tile> GetEnemyRange(Tile origin, float moveRange, int minAttackRange, int maxAttackRange)
    {
        //this might be too slow because iterating over whole list of tiles
        //especially with bigger maps
        List<Tile> tilesInMoveRange = new List<Tile>();

        foreach (Tile t in GridManager.Instance.mapTiles)
        {
            
            List<Tile> distancePath = EnemyPathfind(origin, t);
            distancePath.Remove(origin);
            float moveCost = 0;
            foreach (Tile j in distancePath)
            {
                moveCost += j.GetEnemyMoveCost();

            } 

            if (moveCost <= (int) moveRange && moveCost != 0)
            {
                tilesInMoveRange.Add(t);
            }
        }
        
        List<Tile> tilesInAttackRange = new List<Tile>();

        

        foreach(Tile t in tilesInMoveRange)
        {
            foreach(Tile j in GridManager.Instance.mapTiles)
            {
                int dist = GridManager.Instance.CheckDistance(t, j);
                if(dist <= maxAttackRange && dist >= minAttackRange)
                {
                    if (!tilesInAttackRange.Contains(j))
                    {
                        tilesInAttackRange.Add(j);
                    }
                }
            }
        }

        return tilesInAttackRange;
    }
    
}



    
