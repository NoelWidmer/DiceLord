using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IEntity
{
    GridVector GetStartCoordinates();
    GridVector Coordinates { get; }

    bool CanBeEntered { get; }
    void OnEntered(IEntity entity);

    float Move();

    float Attack();
    void ReceiveDamage(int damage);
    void ReceiveHealth(int health);

    float Ranged();

    bool CanRepell { get; }
    float Repell(IEntity entity);
}

public abstract class Entity : MonoBehaviour, IEntity
{
    private enum State
    {
        Idle,
        Moving,
        Attacking,
        Repelling, 
        Ranged
    }

    private readonly float _attackDuration = .5f;
    private readonly float _rangedDuration = .75f;
    private readonly float _repellDuration = .5f;
    private readonly float _moveDuration = .75f;

    public int X;
    public int Y;

    public GridVector GetStartCoordinates() => new(X, Y);

    public GridVector Coordinates => Grid.Instance.GetCoordiantes(this);

    public Transform Sword;
    public int Health;

    private State _state = State.Idle;
    private GridVector _newCoordiantes;
    private float _remainingMoveDistance;

    private void Awake()
    {
        enabled = false;
        SnapPositionToGrid(GetStartCoordinates());
        HideSword();
    }

    public void ReceiveDamage(int damage)
    {
        if (damage < 1)
        {
            return;
        }

        var newHealth = Health - damage;

        if (newHealth < 1)
        {
            Health = 0;

            Debug.Log($"{name} took {damage} damage and died.");

            var clip = References.Instance.DeathScreamSounds.GetRandomItem();
            PlayParallelSound(clip);

            OnDied();

            Grid.Instance.RemoveEntity(this);

            StartCoroutine(DelayDestroy());

            IEnumerator DelayDestroy()
            {
                yield return new WaitForSeconds(clip.length);
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.Log($"{name} took {damage} damage and has {newHealth} health left.");
            PlayParallelSound(References.Instance.TakeDamageSounds.GetRandomItem());
            Health = newHealth;
        }
    }

    protected abstract void OnDied();

    public void ReceiveHealth(int health)
    {
        if (health < 0)
        {
            return;
        }

        Health += health;
        Debug.Log($"{name} received {health} health and has {Health} health now.");
    }

    private void SnapPositionToGrid(GridVector coordiantes)
    {
        transform.position = coordiantes.GetFieldCenterPosition();
        Debug.Log($"Snap: {name}'s {coordiantes} to {transform.position}");
    }

    public float Attack()
    {
        EnsureState(State.Idle);

        _state = State.Attacking;

        var attackCoordinates = Coordinates.GetAdjacent(GridDirection.NorthEast);

        var targets = Grid.Instance
            .GetEntites(attackCoordinates)
            .ToArray(); // must copy or iterator will throw

        foreach (var target in targets)
        {
            target.ReceiveDamage(1);
        }

        PlayParallelSound(References.Instance.SwordAttackSounds.GetRandomItem());
        ShowSword(attackCoordinates);

        StartCoroutine(DelayEndOffense(_attackDuration));

        return _attackDuration;
    }

    public float Ranged()
    {
        EnsureState(State.Idle);

        _state = State.Ranged;

        var attackCoordinates = Coordinates
            .GetAdjacent(GridDirection.NorthEast)
            .GetAdjacent(GridDirection.NorthEast)
            .GetAdjacent(GridDirection.NorthEast);

        var targets = Grid.Instance
            .GetEntites(attackCoordinates)
            .ToArray(); // must copy or iterator will throw

        foreach (var target in targets)
        {
            target.ReceiveDamage(1);
        }

        PlayParallelSound(References.Instance.RangedSounds.GetRandomItem());
        ShowSword(attackCoordinates);

        StartCoroutine(DelayEndOffense(_rangedDuration));

        return _rangedDuration;
    }

    private List<AudioSource> _audioSources = new();

    private void PlayParallelSound(AudioClip clip)
    {
        var availableSrc = _audioSources.FirstOrDefault(src => src.isPlaying == false);

        if (availableSrc == null)
        {
            availableSrc = gameObject.AddComponent<AudioSource>();
            _audioSources.Add(availableSrc);
        }

        availableSrc.clip = clip;
        availableSrc.Play();
    }

    public abstract bool CanBeEntered { get; }

    public float Move()
    {
        EnsureState(State.Idle);

        var newCoordinates = Coordinates.GetAdjacent(GridDirection.NorthEast);

        var occupants = Grid.Instance
            .GetEntites(newCoordinates)
            .ToArray(); // must copy or iterator will throw

        if (occupants.Length > 0)
        {
            var maxDuration = 0f;

            foreach (var occupant in occupants)
            {
                if (occupant.CanRepell)
                {
                    var duration = occupant.Repell(this);
                    maxDuration = Mathf.Max(maxDuration, duration);
                }
                else if (occupant.CanBeEntered)
                {
                    occupant.OnEntered(this);
                    return DoMove();
                }
                else
                {
                    Debug.Log("move is blocked");
                }
            }

            return maxDuration;
        }
        else
        {
            return DoMove();
        }

        float DoMove()
        {
            PlayParallelSound(References.Instance.PlayerMoveSounds.GetRandomItem());
            _state = State.Moving;
            _newCoordiantes = newCoordinates;
            _remainingMoveDistance = (newCoordinates.GetFieldCenterPosition() - Coordinates.GetFieldCenterPosition()).magnitude;
            enabled = true;
            return _moveDuration;
        }
    }

    public abstract void OnEntered(IEntity entity);

    public abstract bool CanRepell { get; }

    public float Repell(IEntity entity)
    {
        EnsureState(State.Idle);

        _state = State.Repelling;
        entity.ReceiveDamage(1);

        PlayParallelSound(References.Instance.SwordAttackSounds.GetRandomItem());
        ShowSword(entity.Coordinates);

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

            var direction = (_newCoordiantes.GetFieldCenterPosition() - Coordinates.GetFieldCenterPosition()).normalized;
            transform.position += direction * distanceThisFrame;

            if (_state != State.Moving)
            {
                Grid.Instance.UpdateEntityCoordinates(this, _newCoordiantes);
                SnapPositionToGrid(_newCoordiantes);
            }
        }
        else
        {
            throw new InvalidOperationException("Entity should not be updated when not moving.");
        }
    }

    private void ShowSword(GridVector coordinates)
    {
        Sword.position = coordinates.GetFieldCenterPosition();
        Sword.gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }

    private void HideSword()
    {
        if (Sword)
        {
            Sword.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    private void EnsureState(State state)
    {
        if (_state != state)
        {
            throw new InvalidOperationException($"Epected {name} to be in state {state} but was in state {_state}.");
        }
    }

    private IEnumerator DelayEndOffense(float duration)
    {
        yield return new WaitForSeconds(duration);
        _state = State.Idle;
        HideSword();
    }

    private void OnValidate()
    {
        transform.position = GetStartCoordinates().GetFieldCenterPosition();
    }
}
