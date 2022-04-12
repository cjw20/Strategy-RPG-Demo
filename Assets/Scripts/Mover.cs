using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    GameObject unitToMove;
    Tile startTile;
    Tile targetTile;
    float moveDuration; //how long the move should take
    public bool moving = false;

    float elapsedTime;
    float speed = 10f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            
            unitToMove.transform.position = Vector3.Lerp(startTile.transform.position, targetTile.transform.position, speed * Time.deltaTime);

            
            if (unitToMove.transform.position == targetTile.transform.position)
            {
                moving = false;
                elapsedTime = 0;

            }
        }

        //fuck this shit it wont work
    }

    public void SetUpMove(GameObject character, Tile target, float duration)
    {
        unitToMove = character;
        startTile = unitToMove.GetComponent<Unit>().currentTile;
        targetTile = target;
        moveDuration = duration;
        moving = true;
    }
}
