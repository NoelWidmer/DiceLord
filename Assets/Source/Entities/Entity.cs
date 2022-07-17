using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IEntity
{
    int Health { get; }
    Transform Transform { get; }

    GridVector GetStartCoordinates();
    GridVector Coordinates { get; }

    void Move(bool resumeWithNextAction);
    void Melee();
    void Ranged();

    void ReceiveDamage(int damage);
    void ReceiveHealth(int health);

    bool CanBeEntered { get; }
    void OnEntered(IEntity entity);

    bool CanRepell { get; }
    void Repell(IEntity entity);
    void ForceBecomeIdle();
}

public abstract class Entity : MonoBehaviour, IEntity
{
    private enum State
    {
        Idle,

        Moving,
        MoveDirectionRequested,

        Melee,
        MeleeDirectionRequested,

        Ranged,
        RangedDirectionRequested,

        AoE,

        Repelling,
    }

    private readonly float _meleeDuration = .5f;
    private readonly float _repellDuration = .5f;
    private readonly float _moveDuration = .3f;

    private readonly float _rangedChargeDuration = .75f;
    private readonly float _rangedPrepellDuration = .15f;

    private readonly float _aoeDuration = .65f;

    protected virtual int RangeDistance => 2;

    public int X;
    public int Y;
    
    public Transform Transform => transform;

    public GridVector GetStartCoordinates() => new(X, Y);

    public GridVector Coordinates => Grid.Instance.GetCoordiantes(this);

    public GameObject ProjectilePrefab;
    public int Health;

    [Header("Sounds")]
    public AudioClip[] TakeDamageSounds;
    public AudioClip[] DeathSounds;
    public AudioClip[] MeleeSounds;
    public AudioClip[] RangedSounds;
    public AudioClip[] AoESounds;
    public AudioClip[] MoveSounds;

    int IEntity.Health => Health;

    private State _state;

    private GridVector _movingToCoordiantes;
    private float _remainingMoveDistance;

    private AnimationHandler _animHandler;

    protected virtual void Start()
    {
        enabled = false;
        _animHandler = GetComponentInChildren<AnimationHandler>();

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
        _animHandler?.setPlayerWeapon(0);
    }

    private bool _resumeWithNextAction;

    public void Move(bool resumeWithNextAction)
    {
        EnsureState(State.Idle);
        _resumeWithNextAction = resumeWithNextAction;
        _state = State.MoveDirectionRequested;
        _animHandler?.setPlayerWeapon(0);
        OnDirectionalRequest();
    }

    public void Melee()
    {
        EnsureState(State.Idle);
        _state = State.MeleeDirectionRequested;
        _animHandler?.setPlayerWeapon(1);
        OnDirectionalRequest();
    }

    public void Ranged()
    {
        EnsureState(State.Idle);
        _animHandler?.setPlayerWeapon(2);
        _state = State.RangedDirectionRequested;
        OnDirectionalRequest();
    }

    private GameObject SpawnSplash(GridVector coordinates)
    {
        var splash = Instantiate(References.Instance.AttackVisualizerPrefab);
        splash.transform.position = coordinates.GetFieldCenterPosition(); 
        _splashes.Add(splash);
        return splash;
    }

    private void OnDestroy()
    {
        foreach (var splash in _splashes)
        {
            if (splash != null)
            {
                Destroy(splash);
            }
        }
    }

    public void AoE()
    {
        EnsureState(State.Idle);
        _animHandler?.setPlayerWeapon(3);

        _animHandler?.PlayOrStopAttack(true);
        _state = State.AoE;

        var splashes = new List<GameObject>();

        List<IEntity> targets = new();
        foreach(GridDirection direction in Enum.GetValues(typeof(GridDirection)))
        {
            var attackCoordinates = Coordinates.GetAdjacent(direction);

            splashes.Add(SpawnSplash(attackCoordinates));

            targets.AddRange(Grid.Instance
                .GetEntites(attackCoordinates).ToArray()); // must copy or iterator will throw
        }

        foreach (var target in targets)
        {
            target.ReceiveDamage(1);
        }

        if (AoESounds.TryGetRandomItem(out var item))
        {
            this.PlayParallelSound(ref _audioSources, item, true);
        }

        StartCoroutine(DelayEndOffense(_aoeDuration, splashes.ToArray()));
    }

    private List<GameObject> _splashes = new();

    protected abstract void OnDirectionalRequest();

    protected void OnDirectionalResponse(GridDirection direction)
    {
        if (_state == State.MoveDirectionRequested)
        {
            var newCoordinates = Coordinates.GetAdjacent(direction);

            var occupants = Grid.Instance
                .GetEntites(newCoordinates)
                .ToArray(); // must copy or iterator will throw

            if (occupants.Length > 0)
            {
                foreach (var occupant in occupants)
                {
                    if (occupant.CanRepell)
                    {
                        occupant.Repell(this);
                    }
                    else if (occupant.CanBeEntered)
                    {
                        occupant.OnEntered(this);
                        DoMove();
                    }
                    else
                    {
                        _state = State.Idle;
                        this.PlayParallelSound(ref _audioSources, References.Instance.MoveBlockedSound, true);
                        _animHandler?.PlayOrStopMove(false);

                        if (_resumeWithNextAction)
                        {
                            GameMode.Instance.ProcessNextAction();
                        }
                    }
                }
            }
            else
            {
                DoMove();
            }

            void DoMove()
            {
                _animHandler?.PlayOrStopMove(true);

                if (MoveSounds.TryGetRandomItem(out var item))
                {
                    this.PlayParallelSound(ref _audioSources, item, true);
                }

                _state = State.Moving;
                _movingToCoordiantes = newCoordinates;
                _remainingMoveDistance = (newCoordinates.GetFieldCenterPosition() - Coordinates.GetFieldCenterPosition()).magnitude;
                enabled = true;
            }
        }
        else if (_state == State.MeleeDirectionRequested)
        {
            _animHandler?.PlayOrStopAttack(true);
            _state = State.Melee;

            var attackCoordinates = Coordinates.GetAdjacent(direction);

            var targets = Grid.Instance
                .GetEntites(attackCoordinates)
                .ToArray(); // must copy or iterator will throw

            foreach (var target in targets)
            {
                target.ReceiveDamage(2);
            }

            if (MeleeSounds.TryGetRandomItem(out var item))
            {
                this.PlayParallelSound(ref _audioSources, item, true);
            }

            var splash = SpawnSplash(attackCoordinates);

            StartCoroutine(DelayEndOffense(_meleeDuration, new[] { splash }));
        }
        else if (_state == State.RangedDirectionRequested)
        {
            _animHandler?.PlayOrStopAttack(true);
            _state = State.Ranged;

            // spawn projectile
            IProjectile projectile;
            {
                var projectileObj = Instantiate(ProjectilePrefab);
                projectileObj.transform.position = transform.position;
                projectile = projectileObj.GetComponent<Projectile>();
                projectile.Charge(RangeDistance, _rangedPrepellDuration, direction, new AudioClip[0], RangedSounds);
            }
            
            StartCoroutine(DelayEjectProjectile(projectile));

            IEnumerator DelayEjectProjectile(IProjectile projectile)
            {
                yield return new WaitForSeconds(_rangedChargeDuration);
                projectile.Launch();
                StartCoroutine(DelayProjectileHit(Coordinates, RangeDistance - 1, projectile));
            }

            IEnumerator DelayProjectileHit(GridVector fromCoordinates, int remainingProjectileDistance, IProjectile projectile)
            {
                yield return new WaitForSeconds(_rangedPrepellDuration);

                var targetCoordinates = fromCoordinates.GetAdjacent(direction);

                var targets = Grid.Instance
                    .GetEntites(targetCoordinates)
                    .ToArray();

                foreach (var target in targets)
                {
                    if (remainingProjectileDistance == 0)
                    {
                        projectile.OnPartiallyPiercedEntity();
                    }
                    else
                    {
                        projectile.OnPiercedEntity();
                    }

                    target.ReceiveDamage(1);
                }

                if (remainingProjectileDistance == 0 && targets.Length == 0)
                {
                    projectile.OnStuckInEnvironment();
                }

                if (remainingProjectileDistance > 0)
                {
                    StartCoroutine(DelayProjectileHit(targetCoordinates, remainingProjectileDistance - 1, projectile));
                }
                else
                {
                    StartCoroutine(DelayEndOffense(_rangedPrepellDuration, null));
                }
            }
        }
        else
        {
            throw new NotImplementedException("this kind of directional input has not yet been implemented.");
        }
    }

    public abstract bool CanBeEntered { get; }

    public abstract void OnEntered(IEntity entity);

    public abstract bool CanRepell { get; }

    public void Repell(IEntity entity)
    {
        EnsureState(State.Idle);

        _state = State.Repelling;
        entity.ReceiveDamage(1);

        if (MeleeSounds.TryGetRandomItem(out var item))
        {
            this.PlayParallelSound(ref _audioSources, item, true);
        }

        entity.ForceBecomeIdle();

        var splash = SpawnSplash(entity.Coordinates);

        StartCoroutine(DelayEndOffense(_repellDuration, new[] { splash }));
    }
    
    public void ForceBecomeIdle()
    {
        BecomeIdle();
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
            var deltaMove = direction * distanceThisFrame;
            transform.position += new Vector3(deltaMove.x, deltaMove.y, 0f);

            if (_state != State.Moving)
            {
                _animHandler?.PlayOrStopMove(false);
                Grid.Instance.UpdateEntityCoordinates(this, _movingToCoordiantes);
                SnapPositionToGrid(_movingToCoordiantes);

                if (_resumeWithNextAction)
                {
                    GameMode.Instance.ProcessNextAction();
                }
            }
        }
        else
        {
            throw new InvalidOperationException("Entity should not be updated when not moving.");
        }
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
            _animHandler?.PlayOrStopDeath(true);
            Debug.Log($"{name} took {damage} damage and died.");

            var duration = 0f;

            if (DeathSounds.TryGetRandomItem(out var item))
            {
                duration = item.length;
                this.PlayParallelSound(ref _audioSources, item, true);
            }

            Health = 0;

            GameMode.Instance.OnEntityDied(this, duration);

            StartCoroutine(DelayDestroy());

            IEnumerator DelayDestroy()
            {
                yield return new WaitForSeconds(duration);
                Destroy(gameObject);
            }
        }
        else
        {
            _animHandler?.TakeDamage();
            Debug.Log($"{name} took {damage} damage and has {newHealth} health left.");

            if (TakeDamageSounds.TryGetRandomItem(out var item))
            {
                this.PlayParallelSound(ref _audioSources, item, true);
            }

            Health = newHealth;
        }
    }

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

    private IEnumerator DelayEndOffense(float duration, GameObject[] splash)
    {
        yield return new WaitForSeconds(duration);

        if (splash != null)
        {
            foreach (var s in splash)
            {
                Destroy(s);
            }
        }

        BecomeIdle();
        _animHandler?.PlayOrStopAttack(false);
        GameMode.Instance.ProcessNextAction();
    }

    private List<AudioSource> _audioSources = new();

    private void OnValidate()
    {
        transform.position = GetStartCoordinates().GetFieldCenterPosition();
    }
}
