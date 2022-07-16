using UnityEngine;

public class Slime : Entity
{
    public override bool CanBeEntered => false;
    public override bool CanRepell => true;

    public override void OnEntered(IEntity entity)
    { }

    protected override void OnDirectionalRequest()
    { }

    protected override void OnDied()
    { }
}
