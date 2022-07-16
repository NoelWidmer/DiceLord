using System;
using UnityEngine;

public struct GridVector : IEquatable<GridVector>
{
    private const int _fieldPixelWidth = 40;
    private const int _fieldPixelHeight = 20;
    private const int _pixelsPerUnit = 32;

    private const float _fieldWidth = (float)_fieldPixelWidth / _pixelsPerUnit;
    private const float _fieldHeight = (float)_fieldPixelHeight / _pixelsPerUnit;

    public readonly int X;
    public readonly int Y;

    public Vector2 GetFieldCenterPosition()
    {
        var x = new Vector2(X * _fieldWidth * .5f, _fieldHeight * .5f * X);
        var y = new Vector2(Y * _fieldWidth * -.5f, _fieldHeight * .5f * Y);
        return x + y;
    }

    public GridVector(int x, int y)
    {
        X = x;
        Y = y;
    }

    public GridVector GetAdjacent(GridDirection dir) => dir switch
    {
        GridDirection.NorthEast => new GridVector(X + 1, Y),
        GridDirection.SouthWest => new GridVector(X - 1, Y),
        GridDirection.SouthEast => new GridVector(X, Y - 1),
        GridDirection.NorthWest => new GridVector(X, Y + 1),
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