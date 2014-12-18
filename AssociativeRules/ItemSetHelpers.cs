using System;
using System.Collections.Generic;
using System.Linq;

namespace AssociativeRules
{
    public static class ItemSetHelpers
    {
        public static List<ItemSet<T>> SelfJoin<T>(this List<ItemSet<T>> sets) where T : IComparable
        {
            var result = new List<ItemSet<T>>();

            for (var i = 0; i < sets.Count; ++i)
            {
                for (var j = i + 1; j < sets.Count; ++j)
                {
                    var items = sets[i]
                        .Items
                        .Union(sets[j].Items)
                        .ToList();

                    if (items.Count == sets[i].Items.Count + 1)
                    {
                        result.Add(new ItemSet<T>(items));
                    }
                }
            }

            return result;
        }
    }
}