using System.Collections;
using UnityEngine;

public abstract class Enemy : Entity
{
    private GridVector _playerPosition;
    public override bool CanBeEntered => false;

    public override bool CanRepell => true;

    protected abstract void Attack();

    protected override void OnDirectionalRequest()
    {
        GridDirection direction;
        GridVector playerDir = _playerPosition - Coordinates;

        // TODO: Move around corners, don't run into walls!

        GridVector targetDir = playerDir;
        if (targetDir.X == 0)                                       { DirY(targetDir.Y); }
        else if(targetDir.Y == 0)                                   { DirX(targetDir.X); }
        else if(Mathf.Abs(targetDir.X) <= Mathf.Abs(targetDir.Y))   { DirX(targetDir.X); }
        else /* if Y < X */                                         { DirY(targetDir.Y); }

        void DirX(int x)
        {
            if (x > 0) { direction = GridDirection.NorthEast; }
            else { direction = GridDirection.SouthWest; }
        }

        void DirY(int y)
        {
            if (y > 0) { direction = GridDirection.NorthWest; }
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
