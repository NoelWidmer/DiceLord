using UnityEngine;
using UnityEngine.InputSystem;

public interface IPlayerController
{ }

public interface IUnityInputSystemMessages
{
    void OnLogTestMessage(InputValue inputValue);
}

public class PlayerController : Singleton<PlayerController, IPlayerController>, IPlayerController, IUnityInputSystemMessages
{
    public InputActionAsset InputActionAsset;

    private PlayerInput _playerInput;

    protected override void OnAwake()
    {
        _playerInput = gameObject.AddComponent<PlayerInput>();
        _playerInput.actions = InputActionAsset;
    }

    public void OnLogTestMessage(InputValue inputValue)
    {
        Debug.Log("Input system works!");
    }
}
