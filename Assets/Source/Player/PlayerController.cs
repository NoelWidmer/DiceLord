using UnityEngine;
using UnityEngine.InputSystem;

public interface IPlayerController
{
    void Possess(IPlayerCharacter playerCahracter);
}

public interface IUnityInputSystemMessages
{
    void OnRoll(InputValue inputValue);
    void OnMouse(InputValue inputValue);
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
    { }

    private Vector3 _cursorWorldPosition;

    public void OnMouse(InputValue inputValue)
    {
        var cursorPosition = inputValue.Get<Vector2>();

        var cameraPosition = _playerCamera.Camera.transform.position;
        var cursorPosition3d = new Vector3(cursorPosition.x, cursorPosition.y, -cameraPosition.z);
        _cursorWorldPosition = _playerCamera.Camera.ScreenToWorldPoint(cursorPosition3d);
        var direction = (_cursorWorldPosition - cameraPosition).normalized;

        var hit = Physics2D.Raycast(cameraPosition, direction, float.MaxValue);
        if (hit.collider != null)
        {
            Debug.Log("hit");
            if (hit.collider.TryGetComponent<IDirectionalArrowButton>(out var button))
            {
                Debug.Log($"direction: {button.Direction}");
            }
        }
    }
}
