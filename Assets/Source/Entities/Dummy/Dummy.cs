using UnityEngine;

public class Dummy : Entity
{
    public override bool CanBeEntered => false;

    public override bool CanRepell => false;

    protected override AudioClip[] TakeDamageSounds => References.Instance.SlimeTakeDamageSounds;

    protected override AudioClip[] DeathSounds => References.Instance.SlimeScreamSounds;

    public override void OnEntered(IEntity entity)
    { }

    protected override void OnDied()
    { }

    protected override void OnDirectionalRequest()
    { }
}
