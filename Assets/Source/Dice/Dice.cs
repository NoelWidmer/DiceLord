using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : Singleton<Dice, TODO_INTERFACE>
{
    const int NUM_SIDES = 6;
    private GameMode.PlayerAction[] _actions;

    // Start is called before the first frame update
    void Awake()
    {
        _actions = new GameMode.PlayerAction[NUM_SIDES] {
            GameMode.PlayerAction.Move,
            GameMode.PlayerAction.Move,
            GameMode.PlayerAction.Attack,
            GameMode.PlayerAction.NOP,
            GameMode.PlayerAction.Attack,
            GameMode.PlayerAction.Move
        };
    }

    public GameMode.PlayerAction[] RollDice(int n = 1)
    {
        GameMode.PlayerAction[] rolls = new GameMode.PlayerAction[n];
        for(int i = 0; i < n; i++)
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
