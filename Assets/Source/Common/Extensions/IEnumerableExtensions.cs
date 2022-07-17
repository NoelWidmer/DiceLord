using System.Collections.Generic;
using UnityEngine;

public static class IEnumerableExtensions
{
    public static bool TryGetRandomItem<T>(this IReadOnlyList<T> collection, out T item)
    {
        if (collection.Count == 0)
        {
            item = default;
            return false;
        }

        var index = Mathf.FloorToInt(UnityEngine.Random.value * collection.Count);

        if (index == collection.Count)
        {
            index -= 1;
        }

        item = collection[index];
        return true;
    }
}
