using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apriori
{
    public class Apriori<T> where T : IComparable
    {
        private readonly Dictionary<int, int> _supportLevels;
        private readonly List<ItemSet<T>> _analyzableItemSets;
        private readonly double _supportTreshold;
        private readonly double _confidenceTreshold;

        public Apriori(IEnumerable<ItemSet<T>> analyzableItemSets, double supportTreshold, double confidenceTreshold)
        {
            _analyzableItemSets = analyzableItemSets.ToList();
            _supportLevels = new Dictionary<int, int>();
            _supportTreshold = supportTreshold;
            _confidenceTreshold = confidenceTreshold;
        }

        public IEnumerable<AssociativeRule<T>> GetAssociativeRules()
        {
            var frequentItemSets = GetFrequentItemSets();

            return GetAssociativeRules(frequentItemSets);
        }

        public IEnumerable<ItemSet<T>> GetFrequentItemSets()
        {
            var itemSets = new List<ItemSet<T>>();
            var currentItemSets = GetSingleItemSets(_analyzableItemSets);

            while (true)
            {
                var filteredSets = ClearItemSets(currentItemSets);
                if (!filteredSets.Any())
                {
                    break;
                }

                itemSets.AddRange(filteredSets);

                currentItemSets = filteredSets.SelfJoin();
            }

            return itemSets;
        }

        private List<ItemSet<T>> GetSingleItemSets(IEnumerable<ItemSet<T>> itemSets)
        {
            var uniqueItems = new HashSet<T>();
            var items = itemSets.SelectMany(x => x);

            foreach (var item in items)
            {
                if (!uniqueItems.Contains(item))
                {
                    uniqueItems.Add(item);
                }
            }

            return uniqueItems
                .Select(x => new ItemSet<T>(x))
                .ToList();
        }

        private List<ItemSet<T>> ClearItemSets(IEnumerable<ItemSet<T>> itemSets)
        {
            var result = new ConcurrentBag<ItemSet<T>>();
            var totalCount = _analyzableItemSets.Count();

            Parallel.ForEach(itemSets, set =>
            {
                var count = _analyzableItemSets.Count(analyzableSet => analyzableSet.Contains(set));
                if (count / (double)totalCount >= _supportTreshold)
                {
                    result.Add(set);
                    _supportLevels[set.GetHashCode()] = count;
                }
            });

            return result.ToList();
        }

        private IEnumerable<AssociativeRule<T>> GetAssociativeRules(IEnumerable<ItemSet<T>> itemSets)
        {
            var associativeRules = new List<AssociativeRule<T>>();

            foreach (var item in itemSets)
            {
                if (item.Size == 1)
                {
                    continue;
                }

                var subsets = item.GetAllSubsets();
                var itemSupportLevel = _supportLevels[item.GetHashCode()];

                foreach (var subset in subsets)
                {
                    var subsetSupportLevel = _supportLevels[subset.GetHashCode()];

                    var confidence = itemSupportLevel / (double)subsetSupportLevel;

                    if (confidence < _confidenceTreshold)
                    {
                        continue;
                    }

                    associativeRules.Add(new AssociativeRule<T>
                    {
                        Condition = subset,
                        Consequence = item.Subtract(subset)
                    });
                }
            }

            return associativeRules;
        }
    }
}