using System;
using System.Collections.Generic;
using System.Linq;

using OnlineSearchHelper;
using LocalSearchHelper;

namespace FlexyFind
{
    class Program
    {
        private const int maxHits = 10;
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to FlexyFind.");
            Console.Write("Please enter search criteria: ");
            string searchCriteria = Console.ReadLine();

            Console.Write("Enter folder to search. Cannot be the root and only .txt files are supported: ");
            string localPath = Console.ReadLine();
  
            var onlineResults = OnlineSearcher.Search(searchCriteria, maxHits);
            ShowResults("Online results:", onlineResults);

            var localResults = LocalSearcher.Search(localPath, searchCriteria, maxHits);
            ShowResults("Local results:", localResults);

            ShowQueryStats(searchCriteria);
        }

        private static void ShowResults(string header, List<string> results)
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(header);
            Console.WriteLine("-------------------------------------");
  
            foreach (string result in results)
            {
                Console.WriteLine(result);
            }
        }

        private static void ShowQueryStats(string qry)
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Search criteria statistics");
            Console.WriteLine("-------------------------------------");

            Console.WriteLine("Number of alphabets: " + qry.Count(char.IsLetter));
            Console.WriteLine("Number of digits: " + qry.Count(char.IsDigit));
            Console.WriteLine("Number of special characters: " + qry.Count(ch => !char.IsLetterOrDigit(ch)));
        }
    }
}
