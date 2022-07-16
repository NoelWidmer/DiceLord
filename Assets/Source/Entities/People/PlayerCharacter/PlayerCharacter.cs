using UnityEngine;

public interface IPlayerCharacter : IEntity
{
    Vector3 Position { get; }
}

public class PlayerCharacter : Entity, IPlayerCharacter
{
    public Vector3 Position => transform.position;

    public override bool CanBeEntered => false;
    public override bool CanRepell => true;

    protected override void Awake()
    {
        var distanceBetweenFields = Vector2.Distance(Vector2.zero, new GridVector(1, 0).GetFieldCenterPosition());

        var neDirection = new GridVector(1, 0).GetFieldCenterPosition().normalized;
        var nwDirection = new GridVector(0, 1).GetFieldCenterPosition().normalized;

        var arrowNE = transform.Find("Arrow NE");
        arrowNE.transform.position = transform.position + .5f * distanceBetweenFields * neDirection;
        arrowNE.transform.up = neDirection;

        var arrowSE = transform.Find("Arrow SE");
        arrowSE.transform.position = transform.position + .5f * distanceBetweenFields * -nwDirection;
        arrowSE.transform.up = -nwDirection;

        var arrowSW = transform.Find("Arrow SW");
        arrowSW.transform.position = transform.position + .5f * distanceBetweenFields * -neDirection;
        arrowSW.transform.up = -neDirection;

        var arrowNW = transform.Find("Arrow NW");
        arrowNW.transform.position = transform.position + .5f * distanceBetweenFields * nwDirection;
        arrowNW.transform.up = nwDirection;
    }

    public override void OnEntered(IEntity entity)
    { }

    protected override void OnDied()
    {
        GameMode.Instance.OnPlayerCharacterDied();
    }
}
