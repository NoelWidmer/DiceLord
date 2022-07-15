using System;
using UnityEngine;

public struct GridVector : IEquatable<GridVector>
{
    private static readonly Vector2 Half = new(.5f, .5f);

    public readonly int X;
    public readonly int Y;

    public Vector3 FieldCenterPosition
        => Vector2 + Half;

    public Vector2 Vector2
        => new(X, Y);

    public static GridVector From(Vector2 point)
    {
        var x = Mathf.FloorToInt(point.x);
        var y = Mathf.FloorToInt(point.y);
        return new GridVector(x, y);
    }

    public GridVector(int x, int y)
    {
        X = x;
        Y = y;
    }

    public GridVector GetAdjacent(GridDirection dir) => dir switch
    {
        GridDirection.North => new GridVector(1, Y + 1),
        GridDirection.East => new GridVector(X + 1, Y),
        GridDirection.South => new GridVector(X, Y - 1),
        GridDirection.West => new GridVector(X - 1, Y),
        _ => throw new NotImplementedException(),
    };

    private (int, int) _tuple
        => (X, Y);

    public static GridVector operator +(GridVector lhs, GridVector rhs)
        => new(lhs.X + rhs.X, lhs.Y + rhs.Y);

    public static GridVector operator -(GridVector lhs, GridVector rhs)
        => new(lhs.X - rhs.X, lhs.Y - rhs.Y);

    public static bool operator ==(GridVector lhs, GridVector rhs)
        => lhs._tuple == rhs._tuple;

    public static bool operator !=(GridVector lhs, GridVector rhs)
        => lhs._tuple != rhs._tuple;

    public override bool Equals(object obj)
        => obj is GridVector coordiantes && _tuple == coordiantes._tuple;

    public bool Equals(GridVector other)
        => _tuple == other._tuple;

    public override int GetHashCode()
        => _tuple.GetHashCode();

    public override string ToString()
        => _tuple.ToString();
}