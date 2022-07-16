using System;
using UnityEngine;

public struct GridVector : IEquatable<GridVector>
{
    private const int _fieldPixelWidth = 42;
    private const int _fieldPixelHeight = 21;
    private const int _pixelsPerUnit = 32;

    private const float _fieldHeight = (float)_fieldPixelWidth / _pixelsPerUnit;
    private const float _fieldWidth = (float)_fieldPixelHeight / _pixelsPerUnit;

    public readonly int X;
    public readonly int Y;

    public Vector3 FieldCenterPosition
        => new Vector2(X * _fieldWidth *.5f, Y * _fieldHeight);

    public static GridVector From(Vector2 point)
    {
        var x = Mathf.FloorToInt(point.x / _fieldWidth * .5f);

        float adjustedPoint;
        if (x % 2 == 1)
        {
            adjustedPoint = point.y - _fieldHeight * .5f;
        }
        else
        {
            adjustedPoint = point.y;
        }

        var y = Mathf.FloorToInt(adjustedPoint / _fieldHeight * .5f);

        var closestMatch = new GridVector(x, y);
        var distance = Vector2.Distance(closestMatch.FieldCenterPosition, point);

        foreach (GridDirection direction in Enum.GetValues(typeof(GridDirection)))
        {
            var potentialMatch = closestMatch.GetAdjacent(direction);
            var distance2 = Vector2.Distance(potentialMatch.FieldCenterPosition, point);

            if (distance2 < distance)
            {
                closestMatch = potentialMatch;
                distance = distance2;
            }
        }

        return closestMatch;
    }

    public GridVector(int x, int y)
    {
        X = x;
        Y = y;
    }

    public GridVector GetAdjacent(GridDirection dir) => dir switch
    {
        GridDirection.North => new GridVector(X, Y + 1),
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