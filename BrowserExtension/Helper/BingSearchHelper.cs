using BrowserExtension.Enums;
using System;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;

namespace BrowserExtension.Helper
{
    public static class BingSearchHelper
    {
        private const string SubscriptionKey = "faf47c5c8aa3426692ee30c9a05ff472";
        
        public static HttpWebResponse GetSearchResponse(HttpWebRequest request)
        {
            return (HttpWebResponse)request.GetResponse();
        }

        public static HttpWebRequest BuildRequest(string searchFor, int countOfResults, int resultOffset, SearchLanguage searchLanguage, SafeSearchFilter safeSearchFilter)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request parameters
            queryString["q"] = searchFor;
            queryString["count"] = Convert.ToString(countOfResults);
            queryString["offset"] = Convert.ToString(resultOffset);
            queryString["mkt"] = EnumMapper.MapLanguage(searchLanguage);
            queryString["safesearch"] = EnumMapper.MapSafeSearchFilter(safeSearchFilter);
            var requestUri = "https://api.cognitive.microsoft.com/bing/v5.0/search?" + queryString;

            // Create and initialize the request.
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUri);
            request.Headers.Add("Ocp-Apim-Subscription-Key", SubscriptionKey);

            return request;
        }

        public static T GetSearchResultsFrom<T>(HttpWebResponse response)
        {
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                string json = reader.ReadToEnd();
                T searchResults = JsonReaderHelper.ConvertJsonPropertyTo<T>("value", json);

                return searchResults;
            }
        }
    }
}
