using System;
using System.Collections.Generic;
using System.Linq;

namespace Apriori
{
    public static class Utils
    {
        public static List<ItemSet<T>> SelfJoin<T>(this List<ItemSet<T>> sets) where T : IComparable
        {
            var result = new List<ItemSet<T>>();

            for (var i = 0; i < sets.Count; ++i)
            {
                for (var j = i + 1; j < sets.Count; ++j)
                {
                    var items = sets[i]
                        .Union(sets[j])
                        .ToList();

                    if (items.Count == sets[i].Size + 1)
                    {
                        result.Add(new ItemSet<T>(items));
                    }
                }
            }

            return result;
        }
    }
}