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

    private PathNode VectorToNode(GridVector pos)
    {
        return new PathNode(GetEntites(pos).Count > 0, pos);
    }

    public List<GridVector> FindPath(GridVector startPos, GridVector targetPos)
    {
        PathNode startNode = VectorToNode(startPos);
        PathNode targetNode = VectorToNode(targetPos);

        List<PathNode> toSearch = new() { VectorToNode(startPos) };
        List<PathNode> processed = new();

        while (toSearch.Count > 0)
        {
            PathNode current = toSearch[0];

            processed.Add(current);
            toSearch.Remove(current);

            if (current.Equals(targetNode))
            {
                PathNode retraceNode = current;
                List<GridVector> path = new();
                while (retraceNode != startNode)
                {
                    path.Add(retraceNode.Position);
                    retraceNode = retraceNode.Parent;
                }
                return path;
            }

            foreach (GridDirection direction in Enum.GetValues(typeof(GridDirection)))
            {
                PathNode neigborNode = VectorToNode(current.Position.GetAdjacent(direction));
                if (processed.Contains(neigborNode) || neigborNode.IsBlocked)
                    continue;

                int searchIndex = toSearch.IndexOf(neigborNode);
                bool inSearch = searchIndex >= 0;
                if (inSearch)
                { neigborNode = toSearch[searchIndex]; }

                int costToNeighbor = current.GCost + current.GetDistance(neigborNode);

                if (!inSearch || costToNeighbor < neigborNode.GCost)
                {
                    neigborNode.GCost = costToNeighbor;
                    neigborNode.Parent = current;

                    if (!inSearch)
                    {
                        neigborNode.HCost = neigborNode.GetDistance(targetNode);
                        toSearch.Add(neigborNode);
                    }
                }
            }

            toSearch.Sort();
        }
        return null;
    }
}