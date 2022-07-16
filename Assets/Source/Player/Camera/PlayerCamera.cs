using UnityEngine;

public interface IPlayerCamera
{
    Camera Camera { get; }
    void TrackPlayer(IPlayerCharacter playerCharacter);
}

public class PlayerCamera : Singleton<PlayerCamera, IPlayerCamera>, IPlayerCamera
{
    public Camera Camera { get; private set; }

    private IPlayerCharacter _playerCharacter;

    protected override void OnAwake()
    {
        enabled = false;
        Camera = GetComponent<Camera>();
    }

    public void TrackPlayer(IPlayerCharacter playerCharacter)
    {
        _playerCharacter = playerCharacter;
        enabled = true;
    }

    private void LateUpdate()
    {
        var pos = _playerCharacter.Position;
        transform.position = new Vector3(pos.x, pos.y, pos.z - 5f);
    }
}
