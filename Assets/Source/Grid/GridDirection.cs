using System;
using UnityEngine;

public enum GridDirection : byte
{
    NorthEast,
    SouthEast,
    SouthWest, 
    NorthWest
}

public static class GridDirectionExtensions
{
    public static Vector3 GetVector(this GridDirection direction)
    {
        return direction switch
        {
            GridDirection.NorthEast => GridVector.NEDirection,
            GridDirection.SouthEast => -GridVector.NWDirection,
            GridDirection.SouthWest => -GridVector.NEDirection,
            GridDirection.NorthWest => GridVector.NWDirection, 
            _ => throw new NotImplementedException()
        };
    }
}
