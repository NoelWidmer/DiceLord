using UnityEngine;

public interface IGridObject
{
    GridVector GetCoordinatesFromPosition();
}

public class GridObject : MonoBehaviour, IGridObject
{
    public GridVector GetCoordinatesFromPosition() => GridVector.From(transform.position);
}
