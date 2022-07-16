using UnityEngine;

public class Wall : Entity
{
    public override bool CanBeEntered => false;
    public override bool CanRepell => false;

    protected override AudioClip[] TakeDamageSounds => References.Instance.TakeDamageSounds;
    protected override AudioClip[] DeathSounds => References.Instance.DeathScreamSounds;

    public override void OnEntered(IEntity entity)
    { }

    protected override void OnDirectionalRequest()
    { }

    protected override void OnDied()
    { }
}
