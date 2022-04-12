using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private static EnemyManager instance;
    public static EnemyManager Instance { get { return instance; } }

    public List<GameObject> enemies;

    private void Awake()
    {
        instance = this;
    }

    public void StartTurn()
    {
        StartCoroutine(EnemyTurn());
    }
    IEnumerator EnemyTurn()
    {
        //cycles through enemies and takes their turn
        foreach(GameObject e in enemies)
        {
            //status effects
            e.GetComponent<Enemy>().TakeTurn();

            while (e.GetComponent<Enemy>().acting)
            {
                yield return null;
            }
        }
        BattleSystem.Instance.StartAllyTurn();
        yield break;
    }


    public void KilledEnemy(GameObject killed)
    {
        enemies.Remove(killed);

        Destroy(killed);
        if(enemies.Count == 0)
        {
            //end battle if rout condition
            BattleSystem.Instance.EndBattle(GameState.WIN);
        }
    }
}
