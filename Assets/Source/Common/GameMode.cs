using UnityEngine;

public interface IGameMode
{ }

public class GameMode : Singleton<GameMode, IGameMode>, IGameMode
{
    public GameObject ReferencesPrefab;
    public GameObject PlayerControllerPrefab;

    protected override void OnAwake()
    {
        // setup references
        {
            var references = Instantiate(ReferencesPrefab, transform);
            references.name = nameof(References);
        }

        // setup player controller
        {
            var playerController = Instantiate(PlayerControllerPrefab, transform);
            playerController.name = nameof(PlayerController);
        }
    }
}
