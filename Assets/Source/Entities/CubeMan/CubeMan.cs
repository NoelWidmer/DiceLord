using UnityEngine;

public class CubeMan : Enemy
{
    protected override int RangeDistance => 1;
    protected override AudioClip[] TakeDamageSounds => References.Instance.CubeManTakeDamageSounds;
    protected override AudioClip[] DeathSounds => References.Instance.CubeManScreamSounds;

    public override void OnEntered(IEntity entity)
    { }

    protected override void OnDied()
    { }

    protected override void Attack() { Melee(); }
}
