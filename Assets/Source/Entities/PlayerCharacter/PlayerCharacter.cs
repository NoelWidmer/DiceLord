public interface IPlayerCharacter : IEntity
{
}

public class PlayerCharacter : Entity, IPlayerCharacter
{
    protected override void OnDied()
    {
        GameMode.Instance.OnPlayerCharacterDied();
    }
}
