using UnityEngine;

public class Dummy : Enemy
{
    public override bool CanBeEntered => false;

    public override bool CanRepell => false;

    public override void OnEntered(IEntity entity)
    { }

    protected override void Attack()
    { }

    protected override void OnDirectionalRequest()
    { }
}
