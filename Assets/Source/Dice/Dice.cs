using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    const int NUM_SIDES = 6;
    private GameMode.PlayerActionsEnum[] actions;

    // Start is called before the first frame update
    void Start()
    {
        actions = new GameMode.PlayerActionsEnum[NUM_SIDES];
    }

    public void rollDice()
    {
        GameMode.PlayerActionsEnum action = actions[Mathf.FloorToInt(Random.Range(0.0f, NUM_SIDES))];

        switch(action) {
            case GameMode.PlayerActionsEnum.NOP:
                Debug.Log("NOP Action");
                break;
            
            case GameMode.PlayerActionsEnum.Move:
                Debug.Log("Move Action");
                break;

            case GameMode.PlayerActionsEnum.Attack:
                Debug.Log("Attack Action");
                break;
        }
    }

    // Setters and Getters
    public int getNumSides()
    {
        return NUM_SIDES;
    }
}
