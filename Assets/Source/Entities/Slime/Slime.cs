using UnityEngine;

public class Slime : Enemy
{
    protected override int RangeDistance => 1;
    public override bool CanBeEntered => false;
    public override bool CanRepell => true;

    protected override AudioClip[] TakeDamageSounds => References.Instance.SlimeTakeDamageSounds;
    protected override AudioClip[] DeathSounds => References.Instance.SlimeScreamSounds;

    public override void OnEntered(IEntity entity)
    { }

    protected override void Attack() { AoE(); }
}
