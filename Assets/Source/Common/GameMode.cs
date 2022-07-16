using System.Collections;
using UnityEngine;

public interface IGameMode
{
    void OnPlayerCharacterDied();
}

public class GameMode : Singleton<GameMode, IGameMode>, IGameMode
{
    public GameObject ReferencesPrefab;
    public GameObject PlayerControllerPrefab;
    public GameObject DicePrefab;

    public enum PlayerAction
    {
        NOP,
        Move,
        Attack
    }

    private IPlayerCharacter _playerCharacter;
    private DiceController _dice;

    protected override void OnAwake()
    {
        // setup references
        {
            var references = Instantiate(ReferencesPrefab, transform);
            references.name = nameof(References);
        }

        gameObject.AddComponent<Grid>();

        // setup player controller
        {
            var playerController = Instantiate(PlayerControllerPrefab, transform);
            playerController.name = nameof(PlayerController);
        }

        // setup dice
        {
            var diceController = Instantiate(DicePrefab, transform);
            diceController.name = nameof(DiceController);
        }

        // register entities
        {
            IEntity[] entities = FindObjectsOfType<Entity>();
            foreach (var entity in entities)
            {
                Grid.Instance.RegisterEntity(entity);
            }
        }

        _playerCharacter = FindObjectOfType<PlayerCharacter>();
        _dice = FindObjectOfType<DiceController>();
        StartNextTurn();
    }

    private void ProcessAction(PlayerAction action)
    {
        switch(action)
        {
            case PlayerAction.NOP:
                Debug.Log("NOP");
                break;

            case PlayerAction.Move:
                _playerCharacter.Move();
                break;

            case PlayerAction.Attack:
                _playerCharacter.Attack();
                break;
        }
    }

    private void StartNextTurn()
    {
        // roll
        PlayerAction[] actions = _dice.RollDice();
        // choose
        ProcessAction(actions[0]);
        // player act
        // enemy act

        StartCoroutine(DelayNextTurn());
    }

    private IEnumerator DelayNextTurn()
    {
        yield return new WaitForSeconds(3f);
        StartNextTurn();
    }

    public void OnPlayerCharacterDied()
    {
        Debug.Log("Game Over!");
    }
}
