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

    private List<Transform> _directionalArrows = new();

    protected override void Start()
    {
        base.Start();

        {
            var arrowNE = transform.Find("Arrow NE");
            _directionalArrows.Add(arrowNE);

            var offset = .75f * GridVector.DistanceBetweenFields * GridVector.NEDirection;
            arrowNE.transform.position = transform.position + new Vector3(offset.x, offset.y, 0f);
        }

        {
            var arrowSE = transform.Find("Arrow SE");
            _directionalArrows.Add(arrowSE);

            var offset = .75f * GridVector.DistanceBetweenFields * -GridVector.NWDirection;
            arrowSE.transform.position = transform.position + new Vector3(offset.x, offset.y, 0f);
        }

        {
            var arrowSW = transform.Find("Arrow SW");
            _directionalArrows.Add(arrowSW);

            var offset = .75f * GridVector.DistanceBetweenFields * -GridVector.NEDirection;
            arrowSW.transform.position = transform.position + new Vector3(offset.x, offset.y, 0f);
        }

        {
            var arrowNW = transform.Find("Arrow NW");
            _directionalArrows.Add(arrowNW);

            var offset = .75f * GridVector.DistanceBetweenFields * GridVector.NWDirection;
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
}
