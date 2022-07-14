using UnityEngine;

public interface IGameMode
{ }

public class GameMode : Singleton<GameMode, IGameMode>, IGameMode
{
    public GameObject ReferencesPrefab;
    public GameObject PlayerControllerPrefab;

    protected override void OnAwake()
    {
        Instantiate(ReferencesPrefab, transform);
        Instantiate(PlayerControllerPrefab, transform);
    }
}
