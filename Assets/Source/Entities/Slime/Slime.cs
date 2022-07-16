using UnityEngine;

public class Slime : Entity
{
    public override bool CanBeEntered => false;
    public override bool CanRepell => true;

    protected override AudioClip[] TakeDamageSounds => References.Instance.SlimeTakeDamageSounds;
    protected override AudioClip[] DeathSounds => References.Instance.SlimeScreamSounds;

    public override void OnEntered(IEntity entity)
    { }

    protected override void OnDirectionalRequest()
    { }

    protected override void OnDied()
    { }
}
