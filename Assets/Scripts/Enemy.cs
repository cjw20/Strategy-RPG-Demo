using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum BehaviourType { DEFEND, WAIT, AGGRESSIVE, PATROL, RETREAT}
public class Enemy : MonoBehaviour
{
    Unit thisUnit;
    Stats stat;

    public GameObject target;
    Tile targetTile; //tile unit will move towards
    public BehaviourType behavior;

    public List<Tile> patrolPoints;
    int patrolProgress;

    List<Tile> movePath;

    public Tile retreatTile;
    public bool acting; //true when this units turn is taking place


    public int attackRange;
    bool targetInRange; //true when target is in range of attack
    int last; //index of last unoccupied tile in move path

    List<GameObject> alliesInRange;
    List<Tile> tilesInRange;

    Ability chosenAbility;
    // Start is called before the first frame update
    void Start()
    {
        thisUnit = this.gameObject.GetComponent<Unit>();
        stat = this.gameObject.GetComponent<Stats>();
    }

    // Update is called once per frame
    void Update()
    {
        //use for smooth movement?
    }

    void EvaluateBehavior()
    {
        ChooseTarget();
        if(behavior != BehaviourType.DEFEND || behavior != BehaviourType.RETREAT)
        {
            if (targetInRange)
                behavior = BehaviourType.AGGRESSIVE; //chase after target once in range
        }
        //make changes to behavior type if necessary
        if (CheckDistance(target.GetComponent<Unit>().currentTile) < 5) //make 5 variable later
            behavior = BehaviourType.AGGRESSIVE; //chase after target once in range
    }

    void ChooseTarget()
    {
        //chooses target and determines if it is in range of attack
        if (AllyController.Instance.allies.Count == 0)
        {
            return; //end coroutines?
        }

        stat.GetAttackRange();
        alliesInRange = new List<GameObject>();
        tilesInRange = Pathfinding.Instance.GetEnemyRange(thisUnit.currentTile, thisUnit.moveDistance, stat.minAttackRange, stat.maxAttackRange);

        foreach(GameObject a in AllyController.Instance.allies)
        {

            if (tilesInRange.Contains(a.GetComponent<Unit>().currentTile))
            {
                Debug.Log(a.name);
                alliesInRange.Add(a);
            }
        }

        if(alliesInRange.Count != 0)
        {
            int priority = 0;
            foreach(GameObject a in alliesInRange)
            {

                if(a.GetComponent<Ally>().basePriorityValue > priority) //can add calculations to make enemy smarter here
                {
                    target = a;
                    priority = (int) a.GetComponent<Ally>().basePriorityValue;
                }
            }
        }
        else
        {
            int priority = 0;
            foreach (GameObject a in AllyController.Instance.allies) //defaults to all if non in range
            {
                if (a.GetComponent<Ally>().basePriorityValue > priority) //can add calculations to make enemy smarter here
                {
                    target = a;
                    priority = (int)a.GetComponent<Ally>().basePriorityValue;

                }
            }
        }
        
        if(target != null)
        {
            CheckRange();
        }
        
        
    }

    void CheckRange()
    {
        stat.GetAttackRange();
        int dist = CheckDistance(target.GetComponent<Unit>().currentTile);
        if (dist <= stat.maxAttackRange && dist >= stat.minAttackRange) 
        {
            targetInRange = true;
        }
        else
        {
            targetInRange = false;
        }
    }
    int CheckDistance(Tile spot)
    {
        int xDif = Mathf.Abs(spot.xCord - thisUnit.currentTile.xCord);
        int yDif = Mathf.Abs(spot.yCord - thisUnit.currentTile.yCord);

        return yDif + xDif;
    }
    public void TakeTurn()
    {
        acting = true;
        EvaluateBehavior();
        //move based on behavior

        //add support behavior?
        switch (behavior)
        {
            case BehaviourType.DEFEND:
                {
                    StartCoroutine(TakeAction());

                    //chill
                    break;
                }
            case BehaviourType.WAIT:
                {
                    StartCoroutine(TakeAction());
                    break;
                }
            case BehaviourType.AGGRESSIVE:
                {
                    
                    if (targetInRange)
                    {
                        StartCoroutine(TakeAction()); //doesnt move if already in attack range
                        break;
                    }
                    targetTile = Pathfinding.Instance.GetClosestNeighbour(target.GetComponent<Unit>().currentTile, thisUnit.currentTile); 
                    if(targetTile == null)
                    {
                        //target is surrounded, 
                        targetTile = target.GetComponent<Unit>().currentTile.neighbours[0];
                        //default to first neighbour for now
                        //may want to change to true but occupied closet in case neighbours are inaccessable
                        //could also choose new target
                    }
                    
                    movePath = Pathfinding.Instance.Pathfind(thisUnit.currentTile, targetTile);
                    StartCoroutine(Move());
                    
                    break;
                }
            case BehaviourType.PATROL:
                {
                    if(thisUnit.currentTile == patrolPoints[patrolProgress])
                    {
                        patrolProgress++;
                        if(patrolProgress >= patrolPoints.Count)
                        {
                            patrolProgress = 0;
                        }
                    }
                    movePath = Pathfinding.Instance.Pathfind(thisUnit.currentTile, patrolPoints[patrolProgress]);
                    StartCoroutine(Move());
                    //cycle through points
                    break;
                }
            case BehaviourType.RETREAT:
                {
                    movePath = Pathfinding.Instance.Pathfind(thisUnit.currentTile, retreatTile);
                    StartCoroutine(Move());
                    //move to designated retreat point
                    break;
                }
        }
        //separate action switch needed
        
    }

    IEnumerator TakeAction()
    {
        chosenAbility = null;
        List<Ability> possibleActions = stat.abilities;
        possibleActions.Add(stat.basicAttack);
        List<Ability> possibleAttacks = new List<Ability>();
        foreach(Ability a in possibleActions)
        {
            if(!a.support && a.mpCost <= stat.currentMP && a.unlockLevel <= stat.level) //if ability is usable attack
            {
                int dist = GridManager.Instance.CheckDistance(thisUnit.currentTile, target.GetComponent<Unit>().currentTile);
                if (dist <= a.maxRange && dist >= a.minRange)
                {
                    possibleAttacks.Add(a);
                    chosenAbility = a;
                }
            }
        }
        if(possibleAttacks.Count != 0)
        {
            int rand = Random.Range(0, possibleAttacks.Count - 1);
            chosenAbility = possibleAttacks[rand];
            //currently enemies chose randomly from possible attacks
        }
        //support ablility logic goes here

        if(chosenAbility != null)
        {
            chosenAbility.UseAbility(this.gameObject, target);
            Debug.Log(chosenAbility.ToString());
        }
        //look through attack options
        //use a for each loop to decide best action
      
        
        //may need to yield until above finished if coroutines get involved
        acting = false; //after done with action;

        yield break;
    }   
    
    IEnumerator Move()
    {
        FindDestination();  
        
        int x = 0; 
        while (x <= last && x <= movePath.Count - 1)
        {
            thisUnit.transform.position = movePath[x].transform.position;
            if (!movePath[x].enemyOccupied)
            {
                thisUnit.UpdatePosition();
            }

            yield return new WaitForSeconds(.2f);          

            x++;
        }       
        
        StartCoroutine(TakeAction());
        yield break;
    }


    void FindDestination()
    {
        int x = movePath.Count - 1; //index of last tile in list

        float totalCost = 0; //cost to move though each tile in path

        foreach (Tile t in movePath)
        {
            totalCost += t.GetMoveCost();
        }

        if (totalCost > thisUnit.moveDistance)
        {
            while (totalCost > thisUnit.moveDistance)
            {
                totalCost -= movePath[x].GetMoveCost();
                if (totalCost > thisUnit.moveDistance)
                {
                    x--;
                }
                    
            }
        }
        if(x == 0)
        {
            last = x;
            return; //avoids bug where all move tiles are occupied
        }

        
        //x is now index of last reachable tile
        while (movePath[x].enemyOccupied)
        {
            x--;
            if(x == 0)
            {
                break;
            }
        }
        //reduces x until a safe move is found
        last = x;
    }


    

}
