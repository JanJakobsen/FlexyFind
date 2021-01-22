using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace LocalSearchHelper
{    /// <summary>
     /// Static helper class to implement a local file search
     /// </summary>
    static class LocalSearcher
    {
        /// <summary>
        /// Perform a search for txt files in a folder and all it's subfolders limited to a
        /// maximum number of hits
        /// </summary>
        public static List<string> Search(string path, string qry, int maxhits = 5)
        {
            List<string> result = new List<string>();

            try
            {
                result = (from file in Directory.EnumerateFiles(path, "*.txt", SearchOption.AllDirectories)
                          where (File.ReadLines(file).Any(line => line.Contains(qry)))
                          select (file)).Take(maxhits).ToList();

                return result;
            }
            catch (UnauthorizedAccessException)
            {
                result.Add("File access error. Path must be a folder");
                return result;
            }
            catch (Exception e)
            {
                // Smarter and more userfriendly error handling goes here!
                result.Add(e.Message);
                return result;
            }
        }
    }
}
