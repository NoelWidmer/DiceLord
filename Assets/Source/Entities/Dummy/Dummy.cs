using UnityEngine;

public class Dummy : Entity
{
    public override bool CanBeEntered => false;

    public override bool CanRepell => false;

    public override void OnEntered(IEntity entity)
    { }

    protected override void OnDirectionalRequest()
    { }
}
