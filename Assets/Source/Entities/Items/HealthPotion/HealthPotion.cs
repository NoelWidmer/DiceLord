public class HealthPotion : Entity
{
    public override bool CanBeEntered => true;

    public override bool CanRepell => false;

    public override void OnEntered(IEntity entity)
    {
        entity.ReceiveHealth(1);
    }

    protected override void OnDied()
    { }
}
