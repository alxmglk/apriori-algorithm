using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Apriori;

namespace FrequentItemSets
{
    public class Program
    {
        private const double Support = 0.07;

        public static void Main()
        {
            var itemSets = ReadItemSets();
            var apriori = new Apriori<string>(itemSets, Support, 0.1);
            var mostFrequentSet = apriori
                .GetFrequentItemSets()
                .OrderByDescending(x => x.Size)
                .FirstOrDefault();

            if (mostFrequentSet != null)
            {
                Console.WriteLine(mostFrequentSet);
            }

            Console.WriteLine("Execution finished");
            Console.ReadKey();
        }

        private static IEnumerable<ItemSet<string>> ReadItemSets()
        {
            var itemSets = new List<ItemSet<string>>();

            using (var file = new StreamReader(@"App_Data\1.dat"))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    var items = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var set = new ItemSet<string>(items);

                    itemSets.Add(set);
                }
            }

            return itemSets;
        }
    }
}