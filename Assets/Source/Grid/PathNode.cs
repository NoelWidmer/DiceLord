using System;

public class PathNode : IComparable, IEquatable<PathNode>
{
    public GridVector Position;

    public int GCost;
    public int HCost;
    public int FCost => GCost + HCost;

    public bool IsBlocked;
    public PathNode Parent;

    public PathNode(bool _isBlocked, GridVector _pos)
    {
        IsBlocked = _isBlocked;
        Position = _pos;
    }

    public int GetDistance(PathNode other)
    {
        GridVector diff = Position - other.Position;
        return (int) (MathF.Abs(diff.X) + MathF.Abs(diff.Y));
    }

    public int CompareTo(object obj)
    {
        if(obj.GetType() != typeof(PathNode))
            throw new ArgumentException($"{obj} is not of type {typeof(PathNode)}");

        PathNode other = (PathNode) obj;
        int fDiff = FCost - other.FCost;

        if(fDiff != 0)
            { return fDiff; }
        else
            { return HCost - other.HCost; }
    }

    public bool Equals(PathNode other)
    {
        return Position.Equals(other.Position);
    }
}
