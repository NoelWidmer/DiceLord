using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    const int NUM_SIDES = 6;
    private GameMode.PlayerAction[] _actions;

    // Start is called before the first frame update
    void Start()
    {
        _actions = new GameMode.PlayerAction[NUM_SIDES];
        _actions = 
        [
            PlayerAction.Move,
            PlayerAction.Move,
            PlayerAction.Attack,
            PlayerAction.NOP,
            PlayerAction.Attack,
            PlayerAction.Move
        ];
    }

    public GameMode.PlayerAction[] RollDice(int n = 1)
    {
        GameMode.PlayerAction rolls = new GameMode.PlayerAction[n];
        for(int i; i < n; i++)
        {
            rolls[i] = _actions[Mathf.FloorToInt(Random.Range(0.0f, NUM_SIDES))];
        }
        return rolls;
    }

    // Setters and Getters
    public int GetNumSides()
    {
        return NUM_SIDES;
    }
}
