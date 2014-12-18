using System;

namespace Apriori
{
    public class AssociativeRule<T> where T : IComparable
    {
        public ItemSet<T> Condition { get; set; }
        public ItemSet<T> Consequence { get; set; }

        public override string ToString()
        {
            return string.Format("{0} -> {1}", Condition, Consequence);
        }
    }
}