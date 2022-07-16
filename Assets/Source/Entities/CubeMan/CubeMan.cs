using UnityEngine;

public class CubeMan : Entity
{
    public override bool CanBeEntered => false;
    public override bool CanRepell => true;

    protected override AudioClip[] TakeDamageSounds => References.Instance.CubeManTakeDamageSounds;
    protected override AudioClip[] DeathSounds => References.Instance.CubeManScreamSounds;

    public override void OnEntered(IEntity entity)
    { }

    protected override void OnDirectionalRequest()
    { }

    protected override void OnDied()
    { }
}
