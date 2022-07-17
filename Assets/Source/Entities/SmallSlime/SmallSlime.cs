using UnityEngine;

public class SmallSlime : Enemy
{
    public override bool CanBeEntered => false;
    public override bool CanRepell => true;

    protected override AudioClip[] TakeDamageSounds => References.Instance.SlimeSmallTakeDamageSounds;
    protected override AudioClip[] DeathSounds => References.Instance.SlimeSmallScreamSounds;

    public override void OnEntered(IEntity entity)
    { }

    protected override void OnDied()
    { }

    protected override void Attack() { Melee(); /*TODO: AOE*/ }
}
