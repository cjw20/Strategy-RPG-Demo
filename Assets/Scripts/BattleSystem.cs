using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { ALLYTURN, ENEMYTURN, CUTSCENE, WIN, LOSE }
public enum Objective { ROUT, DEFEND, CAPTURE, BOSS} //kill all enemies, dont let enemies reach point(s), capture a tile from enemy, kill boss
public class BattleSystem : MonoBehaviour
{
    private static BattleSystem instance;
    public static BattleSystem Instance { get { return instance; } }

    public GameState gameState;
    public Objective objective;

    public GameObject winPanel;
    public GameObject losePanel;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        StartAllyTurn();
    }


    public void StartAllyTurn()
    {
        gameState = GameState.ALLYTURN;
        BattleUIHandler.Instance.AllyTurn();
        AllyController.Instance.StartTurn();

        //refresh ally turns
    }

    public void StartEnemyTurn()
    {
        gameState = GameState.ENEMYTURN;
        BattleUIHandler.Instance.EnemyTurn();
        EnemyManager.Instance.StartTurn();

        //refresh enemy turns
    }

    public void EndBattle(GameState result)
    {
        if (result == GameState.WIN)
        {
            winPanel.SetActive(true);
            //:)
        }

        else if(result == GameState.LOSE)
        {
            losePanel.SetActive(true);
            //:(
        }
        else
        {
            //????/?
        }
        
    }
}
