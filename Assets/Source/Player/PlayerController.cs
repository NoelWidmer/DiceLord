using UnityEngine;
using UnityEngine.InputSystem;

public interface IPlayerController
{
    void Possess(IPlayerCharacter playerCahracter);
}

public interface IUnityInputSystemMessages
{
    void OnRoll(InputValue inputValue);
}

public class PlayerController : Singleton<PlayerController, IPlayerController>, IPlayerController, IUnityInputSystemMessages
{

    public InputActionAsset InputActionAsset;
    public GameObject PlayerCameraPrefab;

    private PlayerInput _playerInput;
    private IPlayerCamera _playerCamera;

    protected override void OnAwake()
    {
        // setup player input
        {
            _playerInput = gameObject.AddComponent<PlayerInput>();
            _playerInput.actions = InputActionAsset;
            _playerInput.currentActionMap = _playerInput.actions.actionMaps[0];
            _playerInput.currentActionMap.Enable();
        }

        // setup play
        {
            var cameraObject = Instantiate(PlayerCameraPrefab);
            _playerCamera = cameraObject.GetComponent<IPlayerCamera>();
        }
    }

    public void Possess(IPlayerCharacter playerCahracter)
    {
        _playerCamera.TrackPlayer(playerCahracter);
    }

    public void OnRoll(InputValue inputValue)
    {
        
    }
}
