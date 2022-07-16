using System.Collections.Generic;
using UnityEngine;

public static class IEnumerableExtensions
{
    public static T GetRandomItem<T>(this IReadOnlyList<T> collection)
    {
        var index = Mathf.FloorToInt(Random.value * collection.Count);

        if (index == collection.Count)
        {
            index -= 1;
        }

        return collection[index];
    }
}
