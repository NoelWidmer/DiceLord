using System;
using System.Collections;
using UnityEngine;

public interface IEntity
{
    GridVector GetCoordinatesFromPosition();
    float Move();
    float Attack();
}

public class Entity : MonoBehaviour, IEntity
{
    private enum State
    { 
        Idle, 
        Moving, 
        Attacking
    }

    private readonly float _attackDuration = .5f;
    private readonly float _moveDuration = .75f;

    public GridVector GetCoordinatesFromPosition() => GridVector.From(transform.position);

    public Transform _sword;

    private State _state = State.Idle;

    private float _remainingMoveDistance;

    private void Awake()
    {
        enabled = false;
        SnapPositionToGrid();
        _sword.gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void SnapPositionToGrid()
    {
        transform.position = GetCoordinatesFromPosition().FieldCenterPosition;
    }

    public float Attack()
    {
        _state = State.Attacking;
        _sword.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        StartCoroutine(DelayEndAttack());
        return _attackDuration;
    }

    public float Move()
    {
        _state = State.Moving;
        _remainingMoveDistance = 1f;
        enabled = true;
        return _moveDuration;
    }

    private void Update()
    {
        if (_state == State.Moving)
        {
            var distancePerSecond = 1f / _moveDuration;
            var distanceThisFrame = distancePerSecond * Time.deltaTime;

            if (distanceThisFrame >= _remainingMoveDistance)
            {
                distanceThisFrame = _remainingMoveDistance;
                _remainingMoveDistance = 0f;
                _state = State.Idle;
                enabled = false;
            }
            else
            {
                _remainingMoveDistance -= distanceThisFrame;
            }

            transform.position += Vector3.up * distanceThisFrame;

            if (_state != State.Moving)
            {
                SnapPositionToGrid();
                Grid.Instance.UpdateEntityCoordinates(this, GetCoordinatesFromPosition());
            }
        }
        else
        {
            throw new InvalidOperationException("Entity should not be updated when not moving.");
        }
    }

    private IEnumerator DelayEndAttack()
    {
        yield return new WaitForSeconds(_attackDuration);
        _state = State.Idle;
        _sword.gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }
}
