using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Apriori
{
    public class ItemSet<T> : IEnumerable<T> where T : IComparable
    {
        private readonly HashSet<T> _items;

        public ItemSet(IEnumerable<T> items)
        {
            _items = new HashSet<T>(items.Distinct());
        }

        public ItemSet(params T[] items)
        {
            _items = new HashSet<T>(items.Distinct());
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

        public bool Contains(ItemSet<T> itemSet)
        {
            return itemSet.All(x => _items.Contains(x));
        }

        public bool Contains(T item)
        {
            return _items.Contains(item);
        }

        public ItemSet<T> Subtract(ItemSet<T> itemSet)
        {
            var result = _items
                .Where(item => !itemSet.Contains(item))
                .ToList();

            return new ItemSet<T>(result);
        }

        public ItemSet<T> Union(ItemSet<T> itemSet)
        {
            var result = _items.Union(itemSet);

            return new ItemSet<T>(result);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _items.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
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
    }
}