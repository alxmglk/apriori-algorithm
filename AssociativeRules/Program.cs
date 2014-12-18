using System;
using System.Collections.Generic;

namespace AssociativeRules
{
    public class Program
    {
        private const double SupportThreshold = 0.5;
        private const double ConfidenceThreshold = 0.6;

        public static void Main(string[] args)
        {
            var analyzableSets = GetTestItemSets();

            var apriori = new Apriori<string>(analyzableSets, SupportThreshold, ConfidenceThreshold);

            var associativeRules = apriori.GetAssociativeRules();

            foreach (var rule in associativeRules)
            {
                Console.WriteLine(rule.ToString());
            }

            Console.ReadKey();
        }

        private static IEnumerable<ItemSet<string>> GetTestItemSets()
        {
            var itemSets = new List<ItemSet<string>>
                {
                    new ItemSet<string>(new [] { "a", "b", "c" }),
                    new ItemSet<string>(new [] { "b", "a" }),
                    new ItemSet<string>(new [] { "d", "a", "e" })
                };

            return itemSets;
        }
    }
}