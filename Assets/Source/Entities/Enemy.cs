using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Entity
{
    private GridVector _playerPosition;
    public override bool CanBeEntered => false;

    public override bool CanRepell => true;

    protected override AudioClip[] TakeDamageSounds => throw new System.NotImplementedException();

    protected override AudioClip[] DeathSounds => throw new System.NotImplementedException();

    protected abstract void Attack();

    protected override void OnDirectionalRequest()
    {
        GridDirection direction;

        // TODO: Move around corners, don't run into walls!

        GridVector playerDir = _playerPosition - Coordinates;
        if(Mathf.Abs(playerDir.X) <= Mathf.Abs(playerDir.Y))
        {
            if(playerDir.X > 0) { direction = GridDirection.NorthEast; }
            else { direction = GridDirection.SouthWest; }
        }
        else
        {
            if(playerDir.Y > 0) { direction = GridDirection.NorthWest; }
            else { direction = GridDirection.SouthEast; }
        }

        OnDirectionalResponse(direction);
    }

    public void EnemyAct(GridVector playerPosition)
    {
        _playerPosition = playerPosition;

        // if player not in enemy range
        GridVector playerDir = _playerPosition - Coordinates;
        Debug.Log($"Vector between player and enemy {gameObject} is {playerDir}");
        if (!((playerDir.X == 0 && Mathf.Abs(playerDir.Y) <= RangeDistance)
            || (playerDir.Y == 0 && Mathf.Abs(playerDir.X) <= RangeDistance)))
        {
            Move();
            StartCoroutine(DelayAttack(2f));

        }
        else
        {
            Debug.Log($"Player is in range of {gameObject} ({RangeDistance})");
            StartCoroutine(DelayAttack(.1f));
        }

        IEnumerator DelayAttack(float duration)
        {
            yield return new WaitForSeconds(duration);
            Attack();
        }
    }
}
