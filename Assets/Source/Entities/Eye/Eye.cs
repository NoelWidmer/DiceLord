using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eye : Entity
{
    public override bool CanBeEntered => false;
    public override bool CanRepell => true;

    public override void OnEntered(IEntity entity)
    { }

    protected override void OnDirectionalRequest()
    { }

    protected override void OnDied()
    { }
}
