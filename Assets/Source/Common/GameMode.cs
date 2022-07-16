using System.Collections;
using UnityEngine;

public interface IGameMode
{
    void OnPlayerCharacterDied();
}

public class GameMode : Singleton<GameMode, IGameMode>, IGameMode
{
    protected override DisableBehaviour DisableBehaviour => DisableBehaviour.DontDisable;

    public GameObject ReferencesPrefab;
    public GameObject PlayerControllerPrefab;
    public GameObject DicePrefab;

    public int number_of_dice;

    public enum PlayerAction
    {
        NOP,
        Move,
        Melee,
        Ranged,
        AOE,
        Dodge,
        Push
    }

    private IPlayerCharacter _playerCharacter;
    private DiceController _dice;

    protected override void OnAwake()
    {
        _playerCharacter = FindObjectOfType<PlayerCharacter>();

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
            _dice = diceController.GetComponent<DiceController>();
        }

        // register entities
        {
            IEntity[] entities = FindObjectsOfType<Entity>();
            foreach (var entity in entities)
            {
                Grid.Instance.RegisterEntity(entity);
            }
        }

        StartNextTurn();
    }

    private void Start()
    {
        enabled = false;
        PlayerController.Instance.Possess(_playerCharacter);
    }

    private IEnumerator ProcessAction(PlayerAction action)
    {
        switch(action)
        {
            case PlayerAction.NOP:
                Debug.Log("NOP");
                break;

            case PlayerAction.Move:
                yield return new WaitForSeconds(_playerCharacter.Move() + .3f);
                break;

            case PlayerAction.Melee:
                yield return new WaitForSeconds(_playerCharacter.Attack() + .3f);
                break;
        }
    }

    private void StartNextTurn()
    {
        Debug.Log("Top of the round");
        // roll
        PlayerAction[] actions = _dice.RollDice(number_of_dice);
        // choose
        // player act
        foreach(var action in actions)
        {
            StartCoroutine(ProcessAction(action));
        }
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
