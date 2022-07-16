public interface IPlayerCharacter : IEntity
{
}

public class PlayerCharacter : Entity, IPlayerCharacter
{
    public override bool CanBeEntered => false;
    public override bool CanRepell => true;

    public override void OnEntered(IEntity entity)
    { }

    protected override void OnDied()
    {
        GameMode.Instance.OnPlayerCharacterDied();
    }
}
