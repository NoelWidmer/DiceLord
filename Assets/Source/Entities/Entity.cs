using UnityEngine;

public interface IEntity
{
    GridVector GetCoordinatesFromPosition();
    float Move();
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

    private bool _isMoving;
    private float _remainingMoveDistance;

    public float Move()
    {
        Debug.Log("Move (from player character)");
        _isMoving = true;
        _remainingMoveDistance = 1f;
        enabled = true;
        return _moveDuration;
    }

    private float _moveDuration = 2f;

    private void Update()
    {
        if (_isMoving)
        {
            var distancePerSecond = 1f / _moveDuration;
            var distanceThisFrame = distancePerSecond * Time.deltaTime;

            if (distanceThisFrame >= _remainingMoveDistance)
            {
                distanceThisFrame = _remainingMoveDistance;
                _remainingMoveDistance = 0f;
                _isMoving = false;
                enabled = false;
            }
            else
            {
                _remainingMoveDistance -= distanceThisFrame;
            }

            transform.position += Vector3.up * distanceThisFrame;

            if (_isMoving == false)
            {
                Grid.Instance.UpdateEntityCoordinates(this, GetCoordinatesFromPosition());
            }
        }
    }
}
