using UnityEngine;

public class SmallSlime : Enemy
{
    protected override int RangeDistance => 1;
    public override bool CanBeEntered => false;
    public override bool CanRepell => true;

    protected override AudioClip[] TakeDamageSounds => References.Instance.SlimeSmallTakeDamageSounds;
    protected override AudioClip[] DeathSounds => References.Instance.SlimeSmallScreamSounds;

    public override void OnEntered(IEntity entity)
    { }

    protected override void OnDied()
    { }

    protected override void Attack() { Melee(); }
}
