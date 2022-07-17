using UnityEngine;

public class Slime : Enemy
{
    protected override int RangeDistance => 1;
    public override bool CanBeEntered => false;
    public override bool CanRepell => true;

    public override void OnEntered(IEntity entity)
    { }

    protected override void Attack() { AoE(); }
}
