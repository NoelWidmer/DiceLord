using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameMode
{
    void OnPlayerCharacterDied();
    void ProcessNextAction();
}

public class GameMode : Singleton<GameMode, IGameMode>, IGameMode
{
    protected override DisableBehaviour DisableBehaviour => DisableBehaviour.DontDisable;

    public GameObject ReferencesPrefab;
    public GameObject CanvasControllerPrefab;
    public GameObject PlayerControllerPrefab;
    public GameObject DiceControllerPrefab;

    public int number_of_dice;

    private IPlayerCharacter _playerCharacter;
    private DiceController _diceController;
    private CanvasController _canvasController;
    
    private float _timeBuffer = .3f;

    public enum PlayerAction
    {
        NOP,
        Move,
        Melee,
        Ranged,
        AOE,
        Dodge,
        Push,
        Heal
    }

    protected override void OnAwake()
    {
        _playerCharacter = FindObjectOfType<PlayerCharacter>();

        // setup references
        {
            var references = Instantiate(ReferencesPrefab, transform);
            references.name = nameof(References);
        }

        // setup canvas
        {
            var canvasController = Instantiate(CanvasControllerPrefab, transform);
            canvasController.name = nameof(CanvasController);
            _canvasController = canvasController.GetComponent<CanvasController>();
        }

        gameObject.AddComponent<Grid>();

        // setup player controller
        {
            var playerController = Instantiate(PlayerControllerPrefab, transform);
            playerController.name = nameof(PlayerController);
        }

        // setup dice
        {
            var diceController = Instantiate(DiceControllerPrefab, transform);
            diceController.name = nameof(DiceController);
            _diceController = diceController.GetComponent<DiceController>();
        }

        // register entities
        {
            IEntity[] entities = FindObjectsOfType<Entity>();
            foreach (var entity in entities)
            {
                Grid.Instance.RegisterEntity(entity);
            }
        }

        StartCoroutine(DelayFirstTurn());

        IEnumerator DelayFirstTurn()
        {
            yield return new WaitForSeconds(1f);
            StartNextTurn();
        }
    }

    private void Start()
    {
        enabled = false;
        PlayerController.Instance.Possess(_playerCharacter);
    }

    public void ProcessNextAction()
    {
        if (_playerActionIndex == number_of_dice)
        {
            // enemy act (TODO)
            _canvasController.ClearTray();
            StartNextTurn();
        }
        else
        {
            var action = _playerActions[_playerActionIndex];

            switch (action)
            {
                case PlayerAction.NOP:
                    Debug.Log("NOP");
                    StartCoroutine(Wait());

                    IEnumerator Wait()
                    {
                        yield return new WaitForSeconds(.5f);
                        ProcessNextAction();
                    }
                    break;

                case PlayerAction.Move:
                    _playerCharacter.Move();
                    break;

                case PlayerAction.Melee:
                    _playerCharacter.Melee();
                    break;

                case PlayerAction.Ranged:
                    _playerCharacter.Ranged();
                    break;

                case PlayerAction.AOE:
                    break;

                case PlayerAction.Dodge:
                    break;

                case PlayerAction.Push:
                    break;

                case PlayerAction.Heal:
                    break;
            }

            _playerActionIndex += 1;
        }
    }

    private void StartNextTurn()
    {
        Debug.Log("Top of the round");
        // roll
        List<PlayerAction> rolls = _diceController.RollDice(number_of_dice);
        _canvasController.PopulateTray(rolls);

        // choose
        List<PlayerAction> actions = new();
        actions.AddRange(rolls); //TODO
        // player act
        _playerActions = actions;
        _playerActionIndex = 0;
        ProcessNextAction();
    }

    private List<PlayerAction> _playerActions;
    private int _playerActionIndex;

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
