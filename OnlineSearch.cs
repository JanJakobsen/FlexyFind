using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace OnlineSearchHelper
{
    /// <summary>
    /// Static helper class to implement an online web search
    /// </summary>
    static class OnlineSearcher
    {
        private const string bingKey = "ff5eae65dd8542c6809a4e17a97be178";  // TODO: Put this key in an Azure key vault
        private const string bingEndpoint = "https://api.bing.microsoft.com/v7.0/search";

        /// <summary>
        /// Do a web search for a certain criteria and a maximal number of hits
        /// </summary>
        public static List<string> Search(string qry, int maxhits=5)
        {
            try
            {
                // Construct the URI including max number of hits
                var uriQuery = bingEndpoint + "?q=" + Uri.EscapeDataString(qry) + "&count=" + maxhits.ToString();

                // Perform request
                WebRequest request = HttpWebRequest.Create(uriQuery);
                request.Headers["Ocp-Apim-Subscription-Key"] = bingKey;

                // Get response and read it as a json file
                HttpWebResponse response = (HttpWebResponse)request.GetResponseAsync().Result;
                string jsonResponse = new StreamReader(response.GetResponseStream()).ReadToEnd();

                return BuildResultList(jsonResponse);
            }
            catch(Exception e)
            {
                // Smarter and more userfriendly error handling goes here!
                List<string> err = new List<string>();
                err.Add(e.Message);
                return err;
            }
        }

        // Build list of urls from response json file
        private static List<string> BuildResultList(string jsonfile)
        {
            List<string> result = new List<string>();
            Dictionary<string, object> searchResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonfile);

            var webPages = ((Newtonsoft.Json.Linq.JToken)searchResponse["webPages"])["value"];
            if (webPages != null)
            {
                foreach (Newtonsoft.Json.Linq.JToken webpage in webPages)
                {
                    result.Add(webpage["url"].ToString());
                }
            }

            return result;
        }
    }
}
