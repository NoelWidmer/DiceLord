using System;
using System.Collections.Generic;
using UnityEngine;

public static class IEnumerableExtensions
{
    public static T GetRandomItem<T>(this IReadOnlyList<T> collection)
    {
        if (collection.Count == 0)
        {
            throw new NotSupportedException("Cannot get random item if collection is empty.");
        }

        var index = Mathf.FloorToInt(UnityEngine.Random.value * collection.Count);

        if (index == collection.Count)
        {
            index -= 1;
        }

        return collection[index];
    }
}
