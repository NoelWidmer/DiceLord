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
    float Repell(IEntity entity);
}

public class Entity : MonoBehaviour, IEntity
{
    private enum State
    { 
        Idle, 
        Moving, 
        Attacking, 
        Repelling
    }

    private readonly float _attackDuration = .5f;
    private readonly float _repellDuration = .5f;
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

            OnDied();

            Grid.Instance.RemoveEntity(this);
            Destroy(gameObject);
        }
        else
        {
            Debug.Log($"{name} took {damage} damage and has {newHealth} health left.");
            Health = newHealth;
        }
    }

    protected virtual void OnDied()
    { }

    private void Awake()
    {
        enabled = false;
        SnapPositionToGrid();
        HideSword();
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

        ShowSword(attackCoordinates);

        StartCoroutine(DelayEndOffense(_attackDuration));

        return _attackDuration;
    }

    public float Move()
    {
        var newCoordinates = GetCoordinatesFromPosition().GetAdjacent(GridDirection.North);

        var occupants = Grid.Instance
            .GetEntites(newCoordinates)
            .ToArray(); // must copy or iterator will throw

        if (occupants.Length > 0)
        {
            var maxDuration = 0f;

            foreach (var occupant in occupants)
            {
                var duration = occupant.Repell(this);
                maxDuration = Mathf.Max(maxDuration, duration);
            }

            return maxDuration;
        }
        else
        {
            _state = State.Moving;
            _remainingMoveDistance = 1f;
            enabled = true;
            return _moveDuration;
        }
    }

    public float Repell(IEntity entity)
    {
        _state = State.Repelling;
        entity.ReceiveDamage(1);

        ShowSword(entity.GetCoordinatesFromPosition());

        StartCoroutine(DelayEndOffense(_repellDuration));

        return _repellDuration;
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

    private void ShowSword(GridVector coordinates)
    {
        Sword.position = coordinates.FieldCenterPosition;
        Sword.gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }

    private void HideSword()
    {
        Sword.gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    private IEnumerator DelayEndOffense(float duration)
    {
        yield return new WaitForSeconds(duration);
        _state = State.Idle;
        HideSword();
    }
}
