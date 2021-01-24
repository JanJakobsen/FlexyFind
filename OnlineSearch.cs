using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;

namespace OnlineSearchHelper
{
    /// <summary>
    /// Static helper class to implement an online web search
    /// </summary>
    static class OnlineSearcher
    {
        /// <summary>
        /// Do a web search for a certain criteria and a maximal number of hits
        /// </summary>
        public static List<string> Search(string qry, int maxhits=5)
        {
            const string bingEndpoint = "https://api.bing.microsoft.com/v7.0/search";

            try
            {
                // Construct the URI including max number of hits
                var uriQuery = bingEndpoint + "?q=" + Uri.EscapeDataString(qry) + "&count=" + maxhits.ToString();

                // Perform request with subscription key from Azure key vault
                WebRequest request = HttpWebRequest.Create(uriQuery);
                request.Headers["Ocp-Apim-Subscription-Key"] = GetKeyfromVault();

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

        /// <summary>
        /// Get Bing Search key from my Azure key vault
        /// </summary>
        private static string GetKeyfromVault()
        {
            const string keyVault = "https://janskeyvault.vault.azure.net/";
            const string secretName = "BingSearch";

            KeyVaultClient client = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetKeyVaultToken));
            var secret = client.GetSecretAsync(keyVault, secretName).GetAwaiter().GetResult();

            return secret.Value;
        }

        /// <summary>
        /// Give this client access to the key vault
        /// </summary>
        static async Task<string> GetKeyVaultToken(string authority, string resource, string scope)
        {
            const string clientId = "ddd88754-e541-48af-85bc-11778a7cc5f3";
            const string clientSecret = "2qdX.Jm-5w11_2Go5_I_Ex8tq5WMs.0m_~";

            ClientCredential credential = new ClientCredential(clientId, clientSecret);
            var context = new AuthenticationContext(authority, TokenCache.DefaultShared);
            var result = await context.AcquireTokenAsync(resource, credential);

            return result.AccessToken;
        }
    }
}
