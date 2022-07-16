using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public interface IEntity
{
    GridVector GetCoordinatesFromPosition();
    float Move();
    float Attack();
    void ReceiveDamage(int damage);
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

    public Transform Sword;
    public int Health;

    private State _state = State.Idle;
    private float _remainingMoveDistance;


    public void ReceiveDamage(int damage)
    {
        var newHealth = Health - damage;

        if (newHealth < 1)
        {
            Health = 0;

            Debug.Log($"{name} took {damage} damage and died.");

            Grid.Instance.RemoveEntity(this);
            Destroy(gameObject);
        }
        else
        {
            Debug.Log($"{name} took {damage} damage and has {newHealth} health left.");
            Health = newHealth;
        }
    }


    private void Awake()
    {
        enabled = false;
        SnapPositionToGrid();
        Sword.gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void SnapPositionToGrid()
    {
        transform.position = GetCoordinatesFromPosition().FieldCenterPosition;
    }

    public float Attack()
    {
        _state = State.Attacking;

        var attackCoordinates = GetCoordinatesFromPosition().GetAdjacent(GridDirection.North);

        var targets = Grid.Instance
            .GetEntites(attackCoordinates)
            .ToArray(); // must copy or iterator will throw

        foreach (var target in targets)
        {
            target.ReceiveDamage(1);
        }

        Sword.position = attackCoordinates.FieldCenterPosition;
        Sword.gameObject.GetComponent<SpriteRenderer>().enabled = true;

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
        Sword.gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }
}
