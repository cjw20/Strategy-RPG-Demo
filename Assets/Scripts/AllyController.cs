using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ALLYTURNSTATE { MOVE, ATTACK, ITEM, SKILL, TARGET, IDLE }
public class AllyController : MonoBehaviour
{

    private static AllyController instance;
    public static AllyController Instance { get { return instance; } }


    public ALLYTURNSTATE state;
    public List<GameObject> allies;

    public GameObject selectedUnit;
    Unit selectedUnitInfo;
    Ability selectedAbility;
    
    public bool moveable; //true when trying to move an ally unit
    
    List<Tile> movePath;

    List<Tile> moveRange;

    public bool waitingForConfirm;
    public bool canceled;
    public bool actionPlaying; //true when animation/movement is happening
    void Awake()
    {
        instance = this;
        
    }

    public void StartTurn()
    {
        foreach(GameObject a in allies)
        {
            a.GetComponent<Unit>().hasAction = true;
            a.GetComponent<Unit>().hasMove = true;

            //trigger all status effects, cooldowns etc
        }

        state = ALLYTURNSTATE.IDLE;
    }

    public void KilledAlly(GameObject killed)
    {
        allies.Remove(killed);
        killed.SetActive(false); //can be rezzed? stats saved for after combat if no permadeath
        if(allies.Count == 0)
        {
            BattleSystem.Instance.EndBattle(GameState.LOSE);
        }
    }

    public int MoveUnit(Tile destinationTile)
    {
        
        movePath = new List<Tile>(); //clears old path       

        movePath = Pathfinding.Instance.Pathfind(selectedUnitInfo.currentTile, destinationTile);
        Pathfinding.Instance.HideMoveRange();
        StartCoroutine(Move(movePath, (int)selectedUnitInfo.moveDistance, selectedUnit));
        
        
        return 0; //will eventually use for spaces moved
    }

    public void SelectUnit(GameObject unit)
    {
        
        if (state == ALLYTURNSTATE.IDLE)
        {
            selectedUnit = unit;
            selectedUnitInfo = selectedUnit.GetComponent<Unit>();
            BattleUIHandler.Instance.SelectUnit(unit);         
            

        }
      
    }

    public void ShowMove()
    {
        if(state == ALLYTURNSTATE.IDLE && selectedUnitInfo.hasMove)
        {
            state = ALLYTURNSTATE.MOVE;
            moveRange = Pathfinding.Instance.DisplayMoveRange(selectedUnitInfo.currentTile, (int)selectedUnitInfo.moveDistance);
            
            BattleUIHandler.Instance.HideActionWindow();
            BattleUIHandler.Instance.state = UIState.RANGE;
        }
        //grey out move?
    }

    public void ShowAttack()
    {
        if(state == ALLYTURNSTATE.IDLE && selectedUnitInfo.hasAction)
        {
            Ability attack = selectedUnit.GetComponent<Stats>().basicAttack;
            state = ALLYTURNSTATE.ATTACK;
            //GridManager.Instance.DisplayAttackRange(selectedUnitInfo.currentTile, selectedUnitInfo.attackRange, selectedUnitInfo.attackRange);
            attack.DisplayRange(selectedUnit);
            selectedAbility = attack;
            BattleUIHandler.Instance.HideActionWindow();
            BattleUIHandler.Instance.state = UIState.RANGE;
        }        

    }

    public void ShowSkills()
    {
        if(state == ALLYTURNSTATE.IDLE && selectedUnitInfo.hasAction)
        {
            selectedAbility = null; //clears out old ability
            BattleUIHandler.Instance.DisplayAbilities(selectedUnit);
            state = ALLYTURNSTATE.SKILL;
            BattleUIHandler.Instance.state = UIState.RANGE;
        }
    }

    public void ShowAbilityRange(Ability ability)
    {
        state = ALLYTURNSTATE.SKILL;
        selectedAbility = ability;
        ability.DisplayRange(selectedUnit);
        //BattleUIHandler.Instance.HideActionWindow();
        //BattleUIHandler.Instance.HideAbilities();
    }

    public void DeSelectUnit()
    {
        actionPlaying = false;
        Pathfinding.Instance.HideMoveRange();
        GridManager.Instance.HideAttackRange();

        selectedUnit = null;
        selectedUnitInfo = null;
        selectedAbility = null;
        BattleUIHandler.Instance.HideActionWindow();
        state = ALLYTURNSTATE.IDLE; //can now select new unit

    }

    public void EndUnitTurn()
    {
        state = ALLYTURNSTATE.IDLE;
        selectedUnitInfo.hasAction = false;
        selectedUnitInfo.hasMove = false;
        Pathfinding.Instance.HideMoveRange();
        GridManager.Instance.HideAttackRange();
        DeSelectUnit();

        if (CheckForMoves()) //if no moves are left, end turn. may make manual later with ui button
        {
            EndTurn();
        }
        //check if all allies have moved
    }

    bool CheckForMoves()
    {
        bool noMovesLeft = true;
        foreach (GameObject a in allies)
        {
            if(a.GetComponent<Unit>().hasAction || a.GetComponent<Unit>().hasMove)
            {
                noMovesLeft = false;
            }
        }
        return noMovesLeft;
    }

    public void EndTurn()
    {
        DeSelectUnit();
        BattleSystem.Instance.StartEnemyTurn();
    }

    IEnumerator Move(List<Tile> path, int distance, GameObject unit)
    {       
        //store first location for undoing move?       
        int x = 0;
        float usedMove = 0;
        while(usedMove <= distance && x < path.Count)
        {
            unit.transform.position = new Vector3(path[x].transform.position.x, path[x].transform.position.y, 0);

            if (!path[x].allyOccupied)
            {
                selectedUnitInfo.UpdatePosition();
                //so that other ally tiles dont get overwritten when passing through
            }
            
            
            yield return new WaitForSeconds(.2f);
            if(x != path.Count - 1) //so that it doesnt go outside of index when reaching end of path
            {
                usedMove += path[x + 1].GetMoveCost(); //deducts cost of each tile
            }
            
            x++;

        }
        selectedUnitInfo.hasMove = false;        
        //if no action?
        DeSelectUnit();
        
        yield break;
    }


    public void SelectTile(Tile selected)
    {
        switch (state)
        {
            case ALLYTURNSTATE.IDLE:
                {
                    if (selected.allyOccupied)
                    {
                        SelectUnit(selected.unitInTile);
                    }
                    //else display terrain/enemy info
                    break;
                }
            case ALLYTURNSTATE.MOVE:
                {
                    if (selected.inRange) //can only move to if in range
                    {
                        actionPlaying = true;
                        MoveUnit(selected);
                    }
                    else if(!actionPlaying)
                    {
                       // BattleUIHandler.Instance.Back();
                        //DeSelectUnit(); //causes issues if clicked while move is happening
                    }
                    //else cancel move?
                    break;
                }
            case ALLYTURNSTATE.ATTACK:
                {
                    if (selected.inRange && selected.enemyOccupied)
                    {
                        actionPlaying = true;
                        StartCoroutine(WaitForConfirm(selected.unitInTile));                     
                    }
                    else if(!actionPlaying)
                    {
                       // BattleUIHandler.Instance.Back();
                        //DeSelectUnit();
                    }
                    break;
                }
            case ALLYTURNSTATE.ITEM:
                {
                    break;
                }
            case ALLYTURNSTATE.SKILL:
                {
                    
                    if (selected.inRange && selectedAbility != null) // need to add logic for abilities that target allies
                    {
                        if(selectedAbility.support && selected.allyOccupied)
                        {
                            actionPlaying = true;
                            StartCoroutine(WaitForConfirm(selected.unitInTile));
                        }
                        if(!selectedAbility.support && selected.enemyOccupied)
                        {
                            actionPlaying = true;
                            StartCoroutine(WaitForConfirm(selected.unitInTile));
                        }

                        
                    }
                    else if (!actionPlaying)
                    {
                        //BattleUIHandler.Instance.Back();
                        //DeSelectUnit();
                    }
                    break;
                }
            case ALLYTURNSTATE.TARGET:
                {
                    break;
                }
        }
    }

    IEnumerator WaitForConfirm(GameObject target)
    {
        //for multitarget might have to change parameter to array or list
        BattleUIHandler.Instance.ShowAttackWindow(selectedUnit, target, selectedAbility);
        waitingForConfirm = true;
        canceled = true;

        while (waitingForConfirm)
        {
            yield return null;
        }


        if (canceled)
        {
            BattleUIHandler.Instance.HideAttackWindow();
            DeSelectUnit();
            
            SelectUnit(BattleUIHandler.Instance.currentUnit); //brings action window back up
            yield break;
        }

        if (!canceled)
        {
            BattleUIHandler.Instance.HideAttackWindow();
            selectedAbility.UseAbility(selectedUnit, target);
            selectedUnitInfo.hasAction = false;
            DeSelectUnit();
        }
        yield break;
    }

    public void SetConfirm(bool cancel)
    {
        waitingForConfirm = false;
        canceled = cancel;
    }
}
