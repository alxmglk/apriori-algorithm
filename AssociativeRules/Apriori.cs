using System;
using System.Collections.Generic;
using System.Linq;

namespace AssociativeRules
{
    public class Apriori<T>
        where T : IComparable
    {
        private readonly Dictionary<int, int> _supportLevels;
        private readonly IEnumerable<ItemSet<T>> _analyzableItemSets;
        private readonly double _supportTreshold;
        private readonly double _confidenceTreshold;

        public Apriori(IEnumerable<ItemSet<T>> analyzableItemSets, double supportTreshold, double confidenceTreshold)
        {
            _analyzableItemSets = analyzableItemSets;
            _supportLevels = new Dictionary<int, int>();
            _supportTreshold = supportTreshold;
            _confidenceTreshold = confidenceTreshold;
        }

        public IEnumerable<AssociativeRule<T>> GetAssociativeRules()
        {
            var itemSets = new List<ItemSet<T>>();
            var currentItemSets = GetSingleItemSets(_analyzableItemSets);

            while (true)
            {
                var filteredSets = ClearItemSets(currentItemSets, _analyzableItemSets, _supportTreshold, _supportLevels);
                if (!filteredSets.Any())
                {
                    break;
                }

                itemSets.AddRange(filteredSets);

                currentItemSets = ItemSetHelpers.SelfJoin(filteredSets);
            }

            return GetAssociativeRules(itemSets, _confidenceTreshold, _supportLevels);
        }

        private List<ItemSet<T>> GetSingleItemSets(IEnumerable<ItemSet<T>> itemSets)
        {
            IEnumerable<T> distinctItems = new List<T>();

            distinctItems = itemSets.Aggregate(distinctItems, (current, itemSet) => current.Union(itemSet.Items));

            return distinctItems
                .Select(x => new ItemSet<T>(x))
                .ToList();
        }

        private List<ItemSet<T>> ClearItemSets(IEnumerable<ItemSet<T>> itemSets, IEnumerable<ItemSet<T>> analyzableItemSets,
            double supportLevel, Dictionary<int, int> supportLevels)
        {
            var result = new List<ItemSet<T>>();
            var totalCount = analyzableItemSets.Count();

            foreach (var itemSet in itemSets)
            {
                var count = analyzableItemSets.Count(analyzableSet => analyzableSet.Contain(itemSet));

                if (count / (double)totalCount >= supportLevel)
                {
                    result.Add(itemSet);
                    supportLevels[itemSet.GetHashCode()] = count;
                }
            }

            return result;
        }

        private IEnumerable<AssociativeRule<T>> GetAssociativeRules(IEnumerable<ItemSet<T>> itemSets, double confidenceLevel, Dictionary<int, int> supportLevels)
        {
            var associativeRules = new List<AssociativeRule<T>>();

            foreach (var item in itemSets)
            {
                if (item.Size == 1)
                {
                    continue;
                }

                var subsets = item.GetAllSubsets();
                var itemSupportLevel = supportLevels[item.GetHashCode()];

                foreach (var subset in subsets)
                {
                    var subsetSupportLevel = supportLevels[subset.GetHashCode()];

                    var confidence = itemSupportLevel / (double)subsetSupportLevel;

                    if (confidence < confidenceLevel)
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