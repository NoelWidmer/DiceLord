using UnityEngine;

public class SmallSlime : Entity
{
    public override bool CanBeEntered => false;
    public override bool CanRepell => true;

    protected override AudioClip[] TakeDamageSounds => References.Instance.SlimeSmallTakeDamageSounds;
    protected override AudioClip[] DeathSounds => References.Instance.SlimeSmallScreamSounds;

    public override void OnEntered(IEntity entity)
    { }

    protected override void OnDirectionalRequest()
    { }

    protected override void OnDied()
    { }
}
