using UnityEngine;

public interface ISpinner
{
    void ToggleSpinDirection();
}

public class Spinner : Singleton<Spinner, ISpinner>, ISpinner
{
    protected override DisableBehaviour DisableBehaviour => DisableBehaviour.DontDisable;

    private bool _clockwise;

    public void ToggleSpinDirection() => _clockwise = !_clockwise;

    private void Update()
    {
        transform.Rotate(_clockwise ? Vector3.up : -Vector3.up, Time.deltaTime * 100f);
    }
}
