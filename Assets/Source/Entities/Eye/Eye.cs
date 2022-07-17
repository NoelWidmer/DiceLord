using UnityEngine;

public class Eye : Enemy
{
    public override bool CanBeEntered => false;
    public override bool CanRepell => true;

    protected override AudioClip[] TakeDamageSounds => References.Instance.EyeTakeDamageSounds;
    protected override AudioClip[] DeathSounds => References.Instance.EyeScreamSounds;

    public override void OnEntered(IEntity entity)
    { }

    protected override void Attack() { Ranged(); }
}
