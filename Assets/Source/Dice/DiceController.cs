using System;
using System.Collections.Generic;
using UnityEngine;

public interface IDiceController
{
    public List<GameMode.PlayerAction> RollDice(int n);
}

public class DiceController : Singleton<DiceController, IDiceController>, IDiceController
{
    public const int NUM_SIDES = 6;

    private GameMode.PlayerAction[] _actions;

    // Start is called before the first frame update
    protected override void OnAwake()
    {
        _actions = new GameMode.PlayerAction[NUM_SIDES] {
            GameMode.PlayerAction.Move,
            GameMode.PlayerAction.Ranged,
            GameMode.PlayerAction.Melee,
            GameMode.PlayerAction.Ranged,
            GameMode.PlayerAction.Melee,
            GameMode.PlayerAction.Move
        };
    }

    public void SetActions(GameMode.PlayerAction[] actions)
    {
        if(actions.Length > NUM_SIDES)
        {
            throw new ArgumentException($"Actions array should have at most {NUM_SIDES} elements, has {actions.Length}!");
        }
        _actions = actions;
    }

    public List<GameMode.PlayerAction> RollDice(int n = 1)
    {
        List<GameMode.PlayerAction> rolls = new();
        for(int i = 0; i < n; i++)
        {
            if (_actions.TryGetRandomItem(out var item))
            {
                rolls.Add(item);
            };
        }
        return rolls;
    }

    // Setters and Getters
    public int GetNumSides()
    {
        return NUM_SIDES;
    }
}
