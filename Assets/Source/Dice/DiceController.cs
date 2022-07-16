using System.Collections.Generic;
using UnityEngine;

public interface IDiceController
{
    public List<GameMode.PlayerAction> RollDice(int n);
}

public class DiceController : Singleton<DiceController, IDiceController>, IDiceController
{
    const int NUM_SIDES = 6;
    private GameMode.PlayerAction[] _actions;

    // Start is called before the first frame update
    protected override void OnAwake()
    {
        _actions = new GameMode.PlayerAction[NUM_SIDES] {
            GameMode.PlayerAction.Move,
            GameMode.PlayerAction.Move,
            GameMode.PlayerAction.Melee,
            GameMode.PlayerAction.NOP,
            GameMode.PlayerAction.Melee,
            GameMode.PlayerAction.Move
        };

        GetComponentInChildren<Renderer>().material.mainTexture = DiceTextureGenerator.GetTexture();
    }

    public List<GameMode.PlayerAction> RollDice(int n = 1)
    {
        List<GameMode.PlayerAction> rolls = new();
        for(int i = 0; i < n; i++)
        {
            rolls.Add(_actions.GetRandomItem());
        }
        return rolls;
    }

    // Setters and Getters
    public int GetNumSides()
    {
        return NUM_SIDES;
    }
}
