using UnityEngine;

public interface IEntity
{
    GridVector GetCoordinatesFromPosition();
    void Move();
    void Attack();
}

public class Entity : MonoBehaviour, IEntity
{
    public GridVector GetCoordinatesFromPosition() => GridVector.From(transform.position);

    private void Awake()
    {
        transform.position = GetCoordinatesFromPosition().FieldCenterPosition;
    }

    public void Attack()
    {
        Debug.Log("Attack (from player character)");
    }

    public void Move()
    {
        Debug.Log("Move (from player character)");
    }
}
