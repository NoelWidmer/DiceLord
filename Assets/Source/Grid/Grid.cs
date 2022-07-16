using System;
using System.Collections.Generic;
using UnityEngine;

public interface IGrid
{
    void RegisterEntity(IEntity entity);
    IReadOnlyCollection<IEntity> GetEntites(GridVector coordinates);
    GridVector GetCoordiantes(IEntity entity);
    void UpdateEntityCoordinates(IEntity entity, GridVector newCoordinates);
    bool RemoveEntity(IEntity entity);
}

public class Grid : Singleton<Grid, IGrid>, IGrid
{
    private readonly Dictionary<IEntity, GridVector> _entities = new();
    private readonly Dictionary<GridVector, HashSet<IEntity>> _occupiedCoordinates = new();
    private readonly Stack<HashSet<IEntity>> _unusedEntityHashSets = new();

    public void RegisterEntity(IEntity entity)
    {
        var coordinates = entity.GetStartCoordinates();

        if (_entities.TryAdd(entity, coordinates))
        {
            AddEntityToCell(entity, coordinates);
        }
        else
        {
            Debug.LogWarning($"This {entity} has already been registered.");
        }
    }

    public IReadOnlyCollection<IEntity> GetEntites(GridVector coordinates)
        => _occupiedCoordinates.TryGetValue(coordinates, out var entities) ? entities : Array.Empty<IEntity>();

    public GridVector GetCoordiantes(IEntity entity)
        => _entities.TryGetValue(entity, out var coordiantes) ? coordiantes : throw new NotSupportedException("An entity must be registered before accessing it's coordinates.");

    public void UpdateEntityCoordinates(IEntity entity, GridVector newCoordinates)
    {
        if (RemoveEntity(entity))
        {
            AddEntityToCell(entity, newCoordinates);
            _entities[entity] = newCoordinates;
        }
    }

    private void AddEntityToCell(IEntity entity, GridVector coordinates)
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
            _occupiedCoordinates.Add(coordinates, new HashSet<IEntity> { entity });
        }
    }

    public bool RemoveEntity(IEntity entity)
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