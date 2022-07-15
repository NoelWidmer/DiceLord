using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    enum PlayerActionsEnum
    {
        NOP,
        Move,
        Attack
    }    
    
    const int NUM_SIDES = 6;
    private PlayerActionsEnum[] actions;

    // Start is called before the first frame update
    void Start()
    {
        actions = new PlayerActionsEnum[NUM_SIDES];
    }

    public void rollDice()
    {
        PlayerActionsEnum action = actions[Mathf.FloorToInt(Random.Range(0.0f, NUM_SIDES))];

        switch(action) {
            case PlayerActionsEnum.NOP:
                Debug.Log("NOP Action");
                break;
            
            case PlayerActionsEnum.Move:
                Debug.Log("Move Action");
                break;

            case PlayerActionsEnum.Attack:
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
