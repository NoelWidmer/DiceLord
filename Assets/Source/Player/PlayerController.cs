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

    private Vector2 _cursorWorldPosition;

    private IDirectionalArrowButton _hoveringButton;

    public void OnMouse(InputValue inputValue)
    {
        var cursorScreenPosition = inputValue.Get<Vector2>();
        var cursorPosition3d = new Vector3(cursorScreenPosition.x, cursorScreenPosition.y, -_playerCamera.Camera.transform.position.z);
        _cursorWorldPosition = _playerCamera.Camera.ScreenToWorldPoint(cursorPosition3d);

        const float distance = .01f;
        const float halfDistance = distance * .5f;

        var origin = _cursorWorldPosition - Vector2.right * halfDistance;

        var hit = Physics2D.Raycast(origin, Vector2.right, distance);
        if (hit.collider != null)
        {
            if (hit.collider.TryGetComponent<IDirectionalArrowButton>(out var button))
            {
                if (ReferenceEquals(button, _hoveringButton) == false)
                {
                    _hoveringButton = button;
                    button.OnCursorEnter();
                }

                return;
            }
        }

        if (_hoveringButton != null)
        {
            _hoveringButton = null;
        }
    }
}
