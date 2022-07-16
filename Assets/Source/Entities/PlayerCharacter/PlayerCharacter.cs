using System.Collections.Generic;
using UnityEngine;

public interface IPlayerCharacter : IEntity
{
    Vector3 Position { get; }
    void RespondToDirectionalRequest(GridDirection direction);
}

public class PlayerCharacter : Entity, IPlayerCharacter
{
    public Vector3 Position => transform.position;

    public override bool CanBeEntered => false;
    public override bool CanRepell => true;

    protected override AudioClip[] TakeDamageSounds => References.Instance.TakeDamageSounds;
    protected override AudioClip[] DeathSounds => References.Instance.DeathScreamSounds;

    private List<Transform> _directionalArrows = new();

    protected override void Awake()
    {
        base.Awake();

        var distanceBetweenFields = Vector2.Distance(Vector2.zero, new GridVector(1, 0).GetFieldCenterPosition());

        var neDirection = new GridVector(1, 0).GetFieldCenterPosition().normalized;
        var nwDirection = new Vector2(-neDirection.x, neDirection.y);

        {
            var arrowNE = transform.Find("Arrow NE");
            _directionalArrows.Add(arrowNE);

            var offset = .75f * distanceBetweenFields * neDirection;
            arrowNE.transform.position = transform.position + new Vector3(offset.x, offset.y, 0f);
        }

        {
            var arrowSE = transform.Find("Arrow SE");
            _directionalArrows.Add(arrowSE);

            var offset = .75f * distanceBetweenFields * -nwDirection;
            arrowSE.transform.position = transform.position + new Vector3(offset.x, offset.y, 0f);
        }

        {
            var arrowSW = transform.Find("Arrow SW");
            _directionalArrows.Add(arrowSW);

            var offset = .75f * distanceBetweenFields * -neDirection;
            arrowSW.transform.position = transform.position + new Vector3(offset.x, offset.y, 0f);
        }

        {
            var arrowNW = transform.Find("Arrow NW");
            _directionalArrows.Add(arrowNW);

            var offset = .75f * distanceBetweenFields * nwDirection;
            arrowNW.transform.position = transform.position + new Vector3(offset.x, offset.y, 0f);
        }

        ShowArrows(false);
    }

    private void ShowArrows(bool show)
    {
        foreach (var arrow in _directionalArrows)
        {
            arrow.gameObject.SetActive(show);
        }
    }

    protected override void OnDirectionalRequest()
    {
        ShowArrows(true);
    }

    public void RespondToDirectionalRequest(GridDirection direction)
    {
        ShowArrows(false);
        OnDirectionalResponse(direction);
    }

    public override void OnEntered(IEntity entity)
    { }

    protected override void OnDied()
    {
        GameMode.Instance.OnPlayerCharacterDied();
    }
}
