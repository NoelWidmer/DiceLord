using UnityEngine;

public interface IDirectionalArrowButton
{
    GridDirection Direction { get; }
}

public class DirectionalArrowButton : MonoBehaviour
{
    [SerializeField]
    private GridDirection _direction;
    public GridDirection Direction => _direction;

    private void Awake()
    {
        enabled = false;
        var collider = gameObject.AddComponent<BoxCollider2D>();
    }
}
