using UnityEngine;

public class HealthPotion : Entity
{
    public override bool CanBeEntered => true;

    public override bool CanRepell => false;

    protected override AudioClip[] TakeDamageSounds => References.Instance.TakeDamageSounds;
    protected override AudioClip[] DeathSounds => References.Instance.DeathScreamSounds;

    public override void OnEntered(IEntity entity)
    {
        entity.ReceiveHealth(1);
    }

    protected override void OnDirectionalRequest()
    { }

    protected override void OnDied()
    { }
}
