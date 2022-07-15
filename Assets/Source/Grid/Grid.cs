using System;
using System.Collections.Generic;
using UnityEngine;

public interface IGrid
{
    void RegisterEntity(IGridObject entity);
    IReadOnlyCollection<IGridObject> GetEntites(GridVector coordinates);
    GridVector GetCoordiantes(IGridObject entity);
    void UpdateEntityCoordinates(IGridObject entity, GridVector newCoordinates);
}

public class Grid : Singleton<Grid, IGrid>, IGrid
{
    private readonly Dictionary<IGridObject, GridVector> _entities = new();
    private readonly Dictionary<GridVector, HashSet<IGridObject>> _occupiedCoordinates = new();
    private readonly Stack<HashSet<IGridObject>> _unusedEntityHashSets = new();

    public void RegisterEntity(IGridObject entity)
    {
        var coordinates = entity.GetCoordinatesFromPosition();

        if (_entities.TryAdd(entity, coordinates))
        {
            AddEntityToCell(entity, coordinates);
        }
        else
        {
            Debug.LogWarning($"This {entity} has already been registered.");
        }
    }

    public IReadOnlyCollection<IGridObject> GetEntites(GridVector coordinates)
        => _occupiedCoordinates.TryGetValue(coordinates, out var entities) ? entities : Array.Empty<IGridObject>();

    public GridVector GetCoordiantes(IGridObject entity)
        => _entities.TryGetValue(entity, out var coordiantes) ? coordiantes : throw new NotSupportedException("An entity must be registered before accessing it's coordinates.");

    public void UpdateEntityCoordinates(IGridObject entity, GridVector newCoordinates)
    {
        if (RemoveEntityFromCell(entity))
        {
            AddEntityToCell(entity, newCoordinates);
            _entities[entity] = newCoordinates;
        }
    }

    private void AddEntityToCell(IGridObject entity, GridVector coordinates)
    {
        if (_occupiedCoordinates.TryGetValue(coordinates, out var entities))
        {
            if (entities.Add(entity) == false)
            {
                throw new NotSupportedException($"This {entity} is already present at {coordinates}.");
            }
        }
        else if (_unusedEntityHashSets.Count > 0)
        {
            var unusedHashSet = _unusedEntityHashSets.Pop();
            unusedHashSet.Add(entity);

            _occupiedCoordinates.Add(coordinates, unusedHashSet);
        }
        else
        {
            _occupiedCoordinates.Add(coordinates, new HashSet<IGridObject> { entity });
        }
    }

    private bool RemoveEntityFromCell(IGridObject entity)
    {
        if (_entities.TryGetValue(entity, out var coordiantes))
        {
            if (_occupiedCoordinates.TryGetValue(coordiantes, out var entities))
            {
                if (entities.Remove(entity))
                {
                    if (entities.Count == 0)
                    {
                        _occupiedCoordinates.Remove(coordiantes);
                        _unusedEntityHashSets.Push(entities);
                    }

                    return true;
                }
                else
                {
                    throw new NotSupportedException(GetEntityNotFoundErrorMessage());
                }
            }
            else
            {
                throw new NotSupportedException(GetEntityNotFoundErrorMessage());
            }
        }

        Debug.LogWarning($"This {entity} is not registered and can therefore not be removed from its cell.");
        return false;

        string GetEntityNotFoundErrorMessage()
            => $"This {entity} was expected at {coordiantes} but couldn't be found.";
    }
}