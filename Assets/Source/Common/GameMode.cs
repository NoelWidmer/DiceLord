using System.Collections;
using UnityEngine;

public interface IGameMode
{ }

public class GameMode : Singleton<GameMode, IGameMode>, IGameMode
{
    public GameObject ReferencesPrefab;
    public GameObject PlayerControllerPrefab;

    public enum PlayerAction
    {
        NOP,
        Move,
        Attack
    }

    private IPlayerCharacter _playerCharacter;
    private Dice dice;

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

        // register entities
        {
            IEntity[] entities = FindObjectsOfType<Entity>();
            foreach (var entity in entities)
            {
                Grid.Instance.RegisterEntity(entity);
                Debug.Log("registered an entity");
            }
        }

        _playerCharacter = FindObjectOfType<PlayerCharacter>();
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
                Debug.Log("Move");
                _playerCharacter.Move();
                break;

            case PlayerAction.Attack:
                Debug.Log("Attack");
                _playerCharacter.Attack();
                break;
        }
    }

    private void StartNextTurn()
    {
        // roll
        PlayerAction[] actions = dice.RollDice();
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
}
