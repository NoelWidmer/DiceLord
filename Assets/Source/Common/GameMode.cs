using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameMode
{
    void OnPlayerCharacterDied();
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

    private IEnumerator ProcessActions(List<PlayerAction> actions, int idx)
    {
        if(idx == number_of_dice)
        {
            // enemy act (TODO)
            _canvasController.ClearTray();
            StartNextTurn();
            yield break;
        }

        var action = actions[idx];

        switch(action)
        {
            case PlayerAction.NOP:
                yield return new WaitForSeconds(.5f);
                Debug.Log("NOP");
                break;

            case PlayerAction.Move:
                _playerCharacter.Move();
                yield return new WaitForSeconds(1f + _timeBuffer);
                break;

            case PlayerAction.Melee:
                yield return new WaitForSeconds(_playerCharacter.Melee() + _timeBuffer);
                break;

            case PlayerAction.Ranged:
                yield return new WaitForSeconds(_playerCharacter.Ranged() + _timeBuffer);
                break;

            case PlayerAction.AOE:
                yield break;

            case PlayerAction.Dodge:
                yield break;

            case PlayerAction.Push:
                yield break;

            case PlayerAction.Heal:
                yield break;
        }
        Debug.Log("Action Finished");

        StartCoroutine(ProcessActions(actions, idx + 1));
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
        StartCoroutine(ProcessActions(actions, 0));
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
