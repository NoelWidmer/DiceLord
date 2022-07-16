public class Wall : Entity
{
    public override bool CanBeEntered => false;
    public override bool CanRepell => false;

    public override void OnEntered(IEntity entity)
    { }

    protected override void OnDirectionalRequest()
    { }

    protected override void OnDied()
    { }
}
