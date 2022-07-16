using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IEntity
{
    GridVector GetStartCoordinates();
    GridVector Coordinates { get; }

    void Move();

    float Melee();
    float Ranged();

    void ReceiveDamage(int damage);
    void ReceiveHealth(int health);

    bool CanBeEntered { get; }
    void OnEntered(IEntity entity);

    bool CanRepell { get; }
    float Repell(IEntity entity);
}

public abstract class Entity : MonoBehaviour, IEntity
{
    private enum State
    {
        Idle,
        Moving,
        MoveDirectionRequested,
        Melee,
        Repelling, 
        Ranged
    }

    private readonly float _meleeDuration = .5f;
    private readonly float _repellDuration = .5f;
    private readonly float _moveDuration = .75f;

    private readonly float _rangedChargeDuration = .75f;
    private readonly float _rangedPrepellDuration = .25f;
    private readonly int _rangeDistance = 3;

    public int X;
    public int Y;

    public GridVector GetStartCoordinates() => new(X, Y);

    public GridVector Coordinates => Grid.Instance.GetCoordiantes(this);

    public Transform Sword;
    public int Health;

    private State _state;

    private GridVector _movingToCoordiantes;
    private float _remainingMoveDistance;

    protected virtual void Awake()
    {
        enabled = false;
        SnapPositionToGrid(GetStartCoordinates());
        BecomeIdle();
    }

    private void SnapPositionToGrid(GridVector coordiantes)
    {
        transform.position = coordiantes.GetFieldCenterPosition();
    }

    private void BecomeIdle()
    {
        _state = State.Idle;

        if (Sword)
        {
            Sword.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    protected abstract void OnDirectionalRequest();

    public void Move()
    {
        EnsureState(State.Idle);
        _state = State.MoveDirectionRequested;
        OnDirectionalRequest();
    }

    protected float OnDirectionalResponse(GridDirection direction)
    {
        if (_state == State.MoveDirectionRequested)
        {
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
                _movingToCoordiantes = newCoordinates;
                _remainingMoveDistance = (newCoordinates.GetFieldCenterPosition() - Coordinates.GetFieldCenterPosition()).magnitude;
                enabled = true;
                return _moveDuration;
            }
        }
        else
        {
            throw new NotImplementedException("this kind of directional input has not yet been implemented.");
        }
    }

    public float Melee()
    {
        EnsureState(State.Idle);

        _state = State.Melee;

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

        StartCoroutine(DelayEndOffense(_meleeDuration));

        return _meleeDuration;
    }

    public float Ranged()
    {
        EnsureState(State.Idle);

        _state = State.Ranged;

        var direction = GridDirection.NorthEast;
        StartCoroutine(DelayEjectProjectile());

        PlayParallelSound(References.Instance.RangedSounds.GetRandomItem());

        return _rangedChargeDuration + _rangedPrepellDuration * _rangeDistance + 1;

        IEnumerator DelayEjectProjectile()
        {
            yield return new WaitForSeconds(_rangedChargeDuration);
            StartCoroutine(PrepellProjectileTo(Coordinates, _rangeDistance - 1));
        }

        IEnumerator PrepellProjectileTo(GridVector fromCoordinates, int remainingDistance)
        {
            yield return new WaitForSeconds(_rangedPrepellDuration);

            var targetCoordinates = fromCoordinates.GetAdjacent(direction);

            ShowSword(targetCoordinates);

            var targets = Grid.Instance
                .GetEntites(targetCoordinates)
                .ToArray();

            foreach (var target in targets)
            {
                target.ReceiveDamage(1);
            }

            if (remainingDistance > 0)
            {
                StartCoroutine(PrepellProjectileTo(targetCoordinates, remainingDistance - 1));
            }
            else
            {
                StartCoroutine(DelayEndOffense(_rangedPrepellDuration));
            }
        }
    }

    public abstract bool CanBeEntered { get; }

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

            var direction = (_movingToCoordiantes.GetFieldCenterPosition() - Coordinates.GetFieldCenterPosition()).normalized;
            transform.position += direction * distanceThisFrame;

            if (_state != State.Moving)
            {
                Grid.Instance.UpdateEntityCoordinates(this, _movingToCoordiantes);
                SnapPositionToGrid(_movingToCoordiantes);
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

    public void ReceiveDamage(int damage)
    {
        if (damage < 1)
        {
            return;
        }

        var newHealth = Health - damage;

        if (newHealth < 1)
        {
            Debug.Log($"{name} took {damage} damage and died.");

            var clip = References.Instance.DeathScreamSounds.GetRandomItem();
            PlayParallelSound(clip);

            Health = 0;
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
        BecomeIdle();
    }

    private List<AudioSource> _audioSources = new();

    private void PlayParallelSound(AudioClip clip)
    {
        var availableSrc = _audioSources.FirstOrDefault(src => src.isPlaying == false);

        if (availableSrc == null)
        {
            availableSrc = gameObject.AddComponent<AudioSource>();
            _audioSources.Add(availableSrc);

            availableSrc.spatialBlend = 1f;
        }

        availableSrc.clip = clip;
        availableSrc.Play();
    }

    private void OnValidate()
    {
        transform.position = GetStartCoordinates().GetFieldCenterPosition();
    }
}
