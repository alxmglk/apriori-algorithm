using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Apriori
{
    public class ItemSet<T> where T : IComparable
    {
        private readonly List<T> _items;

        public ItemSet(IEnumerable<T> items)
        {
            _items = items.Distinct().ToList();
        }

        public ItemSet(params T[] items)
        {
            _items = items.Distinct().ToList();
        }

        public IList<T> Items
        {
            get
            {
                return new ReadOnlyCollection<T>(_items);
            }
        }

        public int Size
        {
            get { return _items.Count; }
        }

        public IList<ItemSet<T>> GetAllSubsets()
        {
            var result = new List<ItemSet<T>>();
            var maxSetSize = Size - 1;
            var sets = _items
                .Select(x => new ItemSet<T>(x))
                .ToList();

            result.AddRange(sets);

            while (true)
            {
                var selfJoinResult = sets.SelfJoin()
                    .ToList();

                if (selfJoinResult.Any() && selfJoinResult.First().Size > maxSetSize)
                {
                    break;
                }

                result.AddRange(selfJoinResult);
                sets = selfJoinResult;
            }

            return result;
        }

        public bool Contain(ItemSet<T> itemSet)
        {
            foreach (var item in itemSet._items)
            {
                if (_items.All(x => x.CompareTo(item) != 0))
                {
                    return false;
                }
            }

            return true;
        }

        public ItemSet<T> Subtract(ItemSet<T> itemSet)
        {
            var result = new List<T>();

            foreach (var item in _items)
            {
                if (!itemSet.Items.Contains(item))
                {
                    result.Add(item);
                }
            }

            return new ItemSet<T>(result);
        }

        public ItemSet<T> Union(ItemSet<T> itemSet)
        {
            var result = _items.Union(itemSet.Items);

            return new ItemSet<T>(result);
        }

        public override int GetHashCode()
        {
            var hashSum = 0;
            unchecked
            {
                foreach (var item in _items)
                {
                    hashSum += item.GetHashCode();
                }
            }

            return hashSum;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override string ToString()
        {
            return _items.Aggregate(string.Empty, (current, item) => current + (item + " "));
        }

        public static bool operator ==(ItemSet<T> first, ItemSet<T> second)
        {
            return !(first != second);
        }

        public static bool operator !=(ItemSet<T> first, ItemSet<T> second)
        {
            return !(first.GetHashCode() == second.GetHashCode());
        }
    }
}