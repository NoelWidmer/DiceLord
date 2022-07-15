using System.Collections;
using UnityEngine;

public interface IGameMode
{ }

public class GameMode : Singleton<GameMode, IGameMode>, IGameMode
{
    public GameObject ReferencesPrefab;
    public GameObject PlayerControllerPrefab;

    public enum PlayerActionsEnum
    {
        NOP,
        Move,
        Attack
    }

    private IPlayerCharacter _playerCharacter;

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

    private void StartNextTurn()
    {
        // roll
        // choose
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
