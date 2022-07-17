using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum TurnState
{
    Roll,
    Choose,
    PlayerAct,
    EnemyAct
}

public interface IGameMode
{
    TurnState TurnState { get; }
    void OnEntityDied(IEntity entity, float deathDuration);
    void ProcessNextAction();
}

public class GameMode : Singleton<GameMode, IGameMode>, IGameMode
{
    protected override DisableBehaviour DisableBehaviour => DisableBehaviour.DontDisable;

    public GameObject ReferencesPrefab;
    public GameObject CanvasControllerPrefab;
    public GameObject PlayerControllerPrefab;
    public GameObject DiceControllerPrefab;
    public AudioClip AmbientTrack;

    public int number_of_dice;
    public const int number_of_sides = 6;
    public PlayerAction[] sides = new PlayerAction[number_of_sides];
    public bool overrideDice = false;

    private IPlayerCharacter _playerCharacter;
    private IDiceController _diceController;
    private CanvasController _canvasController;

    private List<Enemy> _enemies;

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

    /*********************************
     * Init
     *********************************/
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
        bool _firstInstance = false;
        if(DiceController.Instance == null)
        {
            _firstInstance = true;
            var diceController = new GameObject("Dice Controller");
            diceController.AddComponent<DiceController>();

            DontDestroyOnLoad(diceController);
        }

        {
            _diceController = DiceController.Instance;

            List<PlayerAction> actions = new(sides);
            if (overrideDice || _firstInstance) { _diceController.SetActions(actions); }
        }

        // setup ambient track
        if (AmbientController.Instance == null)
        {
            var ambientObj = new GameObject("Ambient Track");
            ambientObj.AddComponent<AmbientController>();

            DontDestroyOnLoad(ambientObj);

            var src = ambientObj.AddComponent<AudioSource>();
            src.clip = AmbientTrack;
            src.volume = .2f;
            src.loop = true;
            src.Play();
        }

        if (SceneTracker.Instance == null)
        {
            var sceneTrackerObj = new GameObject("Scene Tracker");
            sceneTrackerObj.AddComponent<SceneTracker>();

            DontDestroyOnLoad(sceneTrackerObj);
        }

        // register entities
        {
            _enemies = new();
            IEntity[] entities = FindObjectsOfType<Entity>();
            foreach (var entity in entities)
            {
                Grid.Instance.RegisterEntity(entity);
                if (entity.GetType().IsSubclassOf(typeof(Enemy)))
                {
                    _enemies.Add((Enemy)entity);
                }
            }

            Debug.Log($"Registered {_enemies.Count} enemies");
        }

        StartCoroutine(DelayFirstTurn());

        IEnumerator DelayFirstTurn()
        {
            yield return new WaitForSeconds(.25f);
            StartNextTurn();
        }
    }

    public void OnEntityDied(IEntity entity, float deathDuration)
    {
        Grid.Instance.RemoveEntity(entity);

        if (entity is Enemy enemy)
        {
            _enemies.Remove(enemy);
            _queuedEnemies.Remove(enemy);

            if (_enemies.Count == 0)
            {
                SceneTracker.Instance.SetLastScene(gameObject.scene.buildIndex);
                StartCoroutine(DelaySceneLoad(1));
            }
        }
        else if (entity is PlayerCharacter)
        {
            StartCoroutine(DelaySceneLoad(0));
        }

        IEnumerator DelaySceneLoad(int sceneIndex)
        {
            yield return new WaitForSeconds(deathDuration);
            SceneManager.LoadScene(sceneIndex);
        }
    }

    private void Start()
    {
        enabled = false;
        PlayerController.Instance.Possess(_playerCharacter);
    }

    /*********************************
     * Turn State Machine
     *********************************/

    public TurnState TurnState { get; private set; }

    private void StartNextTurn()
    {
        Debug.Log("Top of the round");
        TurnState = TurnState.Roll;
        _canvasController.EnableRollButton(true);
    }

    public void OnRollDice()
    {
        if(TurnState != TurnState.Roll)
        {
            throw new InvalidOperationException("We're not in the Roll state, but you tried to roll");
        }

        _canvasController.EnableRollButton(false);
        List<PlayerAction> rolls = _diceController.RollDice(number_of_dice);
        _canvasController.PopulateTray(rolls);

        TurnState = TurnState.Choose;
        _canvasController.EnableConfirmButton(true);
    }

    public void OnConfirmSelection()
    {
        if (TurnState != TurnState.Choose)
        {
            throw new InvalidOperationException("We're not in the Choose state, but you tried to confirm");
        }

        _canvasController.EnableConfirmButton(false);
        _playerActions = _canvasController.GetSelectedActions();
        _playerActionIndex = 0;

        _queuedEnemies = _enemies.ToList(); // make copy
        _enemyActionsStarted = false;

        TurnState = TurnState.PlayerAct;
        ProcessNextAction();
    }

    private bool _enemyActionsStarted;
    private List<Enemy> _queuedEnemies;
    private List<PlayerAction> _playerActions;
    private int _playerActionIndex;

    public void ProcessNextAction()
    {
        if (_playerActionIndex >= _playerActions.Count)
        {
            if (_enemyActionsStarted == false)
            {
                // here are the things we only want to do once
                _enemyActionsStarted = true;

                if (_playerActionIndex < number_of_dice)
                {
                    // only move slot indicator, execute no action (polish)
                }

                _canvasController.ClearTray();
                _canvasController.ClearSlots();

                // enemy act
                TurnState = TurnState.EnemyAct;
            }

            _queuedEnemies.RemoveAll(e => e == null);

            if (_queuedEnemies.Count > 0)
            {
                var nextEnemy = _queuedEnemies.First();

                _queuedEnemies.Remove(nextEnemy);

                if (nextEnemy.Health > 0)
                {
                    nextEnemy.EnemyAct(_playerCharacter.Coordinates);
                }
                else
                {
                    ProcessNextAction();
                }
            }
            else
            {
                // we might start a new round too early if the killed enemy (in the current round) has dissapeard but we are still processing action.
                // depends on how long our rounds take.
                StartNextTurn();
            }
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
                    _playerCharacter.Move(true);
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
}
