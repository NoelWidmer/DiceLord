using UnityEngine;

public class InvisibleWall : Entity
{
    public override bool CanBeEntered => false;
    public override bool CanRepell => false;

    protected override void Start()
    {
        base.Start();
        var renderer = GetComponentInChildren<SpriteRenderer>();
        Destroy(renderer);
    }

    public override void OnEntered(IEntity entity)
    { }

    protected override void OnDirectionalRequest()
    { }
}
