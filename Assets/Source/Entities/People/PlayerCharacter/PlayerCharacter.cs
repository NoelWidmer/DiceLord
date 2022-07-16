using UnityEngine;

public interface IPlayerCharacter : IEntity
{
    Vector3 Position { get; }
}

public class PlayerCharacter : Entity, IPlayerCharacter
{
    public Vector3 Position => transform.position;

    public override bool CanBeEntered => false;
    public override bool CanRepell => true;

    public override void OnEntered(IEntity entity)
    { }

    protected override void OnDied()
    {
        GameMode.Instance.OnPlayerCharacterDied();
    }
}
