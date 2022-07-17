using UnityEngine;

public class CubeMan : Enemy
{
    protected override int RangeDistance => 1;

    public override void OnEntered(IEntity entity)
    { }

    protected override void Attack() { Melee(); }
}
