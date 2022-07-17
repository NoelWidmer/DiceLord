using UnityEngine;

public class Eye : Enemy
{
    protected override int RangeDistance => 3;
    public override bool CanBeEntered => false;
    public override bool CanRepell => true;

    public override void OnEntered(IEntity entity)
    { }

    protected override void Attack() { Ranged(); }
}
