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
    public GameObject PlayerControllerPrefab;
    public GameObject DiceControllerPrefab;

    public int number_of_dice;

    public GameObject SlotPrefab;

    private GameObject _canvas;
    private GameObject _tray;
    private GameObject _slotsArea;

    private IPlayerCharacter _playerCharacter;
    private DiceController _dice;

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
            var diceController = Instantiate(DiceControllerPrefab, transform);
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

        // UI
        InitCanvas();

        StartNextTurn();
    }

    private void InitCanvas()
    {
        // init canvas
        _canvas = transform.Find("Canvas").gameObject;


        // init tray
        _tray = _canvas.transform.Find("Tray").gameObject;

        // init slots
        _slotsArea = _canvas.transform.Find("SlotsArea").gameObject;
        float areaWidth = _slotsArea.GetComponent<RectTransform>().rect.width;
        for (int i = 0; i < number_of_dice; i++)
        {
            var slot = Instantiate(SlotPrefab, transform);
            slot.name = "Slot " + i;
            slot.transform.SetParent(_slotsArea.transform);
            float width = slot.GetComponent<RectTransform>().rect.width;
            float offset = areaWidth / 2 - width / 2;
            slot.GetComponent<RectTransform>().localPosition = new(100f * i - offset, 0f);
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
                yield return new WaitForSeconds(_playerCharacter.Move() + .3f);
                break;

            case PlayerAction.Melee:
                yield return new WaitForSeconds(_playerCharacter.Attack() + .3f);
                break;
        }
        Debug.Log("Action Finished");

        StartCoroutine(ProcessActions(actions, idx + 1));
    }

    private void StartNextTurn()
    {
        Debug.Log("Top of the round");
        // roll
        List<PlayerAction> rolls = _dice.RollDice(number_of_dice);
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
