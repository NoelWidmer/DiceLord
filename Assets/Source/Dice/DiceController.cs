using System;
using System.Collections.Generic;
using UnityEngine;

public interface IDiceController
{
    public List<GameMode.PlayerAction> RollDice(int n);
    public void SetActions(List<GameMode.PlayerAction> actions);
    public int GetNumSides();
}

public class DiceController : Singleton<DiceController, IDiceController>, IDiceController
{
    public const int NUM_SIDES = 6;

    private List<GameMode.PlayerAction> _actions;

    // Start is called before the first frame update
    protected override void OnAwake()
    {
        _actions = new();
    }

    public void SetActions(List<GameMode.PlayerAction> actions)
    {
        if(actions.Count > NUM_SIDES)
        {
            throw new ArgumentException($"Actions array should have at most {NUM_SIDES} elements, has {actions.Count}!");
        }
        _actions = actions;
    }

    public List<GameMode.PlayerAction> GetDiceActions() => _actions;

    public List<GameMode.PlayerAction> RollDice(int n = 1)
    {
        List<GameMode.PlayerAction> rolls = new();
        for(int i = 0; i < n; i++)
        {
            rolls.Add(_actions.GetRandomItem());
        }
        return rolls;
    }

    public int GetNumSides() => NUM_SIDES;
}
