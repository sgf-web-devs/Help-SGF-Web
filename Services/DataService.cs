using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Algolia.Search;
using HelpSGFWebApp.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;

namespace HelpSGFWebApp.Services
{
    public class DataService
    {
        static HttpClient httpClient = new HttpClient();
        public List<Location> Locations { get; set; }
        public List<LocationSearchResultItem> LocationSearchResultItems { get; set; }
        
        public static string AlgoliaApplicationID = "";
        public static string AlgoliaApiKey = "";
        public static string AlgoliaIndex = "";
        public static string HelpSGFAPIRoot = "";
        
        private AlgoliaClient _client;
        private Index _index;

        public DataService()
        {
            AlgoliaApplicationID = ConfigurationManager.AppSetting["AlgoliaApplicationID"];
            AlgoliaApiKey = ConfigurationManager.AppSetting["AlgoliaApiKey"];
            AlgoliaIndex = ConfigurationManager.AppSetting["AlgoliaIndex"];
            HelpSGFAPIRoot = ConfigurationManager.AppSetting["HelpSGFAPIRoot"];

            _client = new AlgoliaClient(AlgoliaApplicationID, AlgoliaApiKey);
            _index = _client.InitIndex(AlgoliaIndex);
        }
        
        public Dictionary<string, Dictionary<string, int>> GetFacets()
        {
            var query = new Query("");
            query.SetNbHitsPerPage(0);
            query.SetFacets(new List<string> {"categories.lvl0", "categories.lvl1" });
            var res = _index.Search(query);
            var results = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, int>>>(res["facets"].ToString());

            return results;
        }
        
        public async Task<List<MainCategory>> GetMainCategoriesAsync()
        {
            var path = HelpSGFAPIRoot + "/Umbraco/Api/API/GetMainCategories";
            HttpResponseMessage response = await httpClient.GetAsync(path);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                var categories = JsonConvert.DeserializeObject<List<MainCategory>>(json);

                return categories;
            }

            return null;
        }
        
        public List<LocationSearchResultItem> GetLocations()
        {
            var i = 0;

            var res = _index.Search(new Query("food"));
            var count = res.Count;
            var results = JsonConvert.DeserializeObject<List<LocationSearchResultItem>>(res["hits"].ToString());


            LocationSearchResultItems = new List<LocationSearchResultItem>();

            var locations = new List<LocationSearchResultItem>();

            Locations.Clear();

            foreach (var location in results)
            {
                i++;
                location.Index = i;
                LocationSearchResultItems.Add(location);
            }

            return LocationSearchResultItems;
        }

        public List<LocationSearchResultItem> SearchLocations(string searchText)
        {
            var i = 0;


            var res = _index.Search(new Query(searchText));

            var count = res.Count;
            var results = JsonConvert.DeserializeObject<List<LocationSearchResultItem>>(res["hits"].ToString());

            LocationSearchResultItems = new List<LocationSearchResultItem>();

            //Locations.Clear();

            foreach (var location in results)
            {
                i++;
                location.Index = i;
                LocationSearchResultItems.Add(location);
            }

            return LocationSearchResultItems;
        }

        public List<LocationSearchResultItem> FilterLocations(string subCategory, int level = 1)
        {
            var i = 0;
            
            var categoryLevelFilter = "categories.lvl" + level + ":";
            
            var query = new Query();
            //query.SetFacetFilters(new[] { "categories.lvl1:" + subCategory });
            query.SetFacetFilters(new[] { categoryLevelFilter + subCategory });

            var res = _index.Search(query);

            var count = res.Count;
            var results = JsonConvert.DeserializeObject<List<LocationSearchResultItem>>(res["hits"].ToString());

            LocationSearchResultItems = new List<LocationSearchResultItem>();

            foreach (var location in results)
            {
                i++;
                location.Index = i;
                LocationSearchResultItems.Add(location);
            }

            return LocationSearchResultItems;
        }
        
        public async Task<Location> GetLocationAsync(string urlPath)
        {
            var location = new Location();
            
            urlPath = urlPath.Replace("/", "");
            urlPath += "/";
            
            var path = HelpSGFAPIRoot + urlPath + "json";

            HttpResponseMessage response = await httpClient.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                //location = await response.Content.ReadAsStringAsync<Location>();
                var json = await response.Content.ReadAsStringAsync();

                location = JsonConvert.DeserializeObject<Location>(json);

                return location;
            }

            return null;
        }
        
        public Location GetLocation(string urlPath)
        {
            var client = new RestClient(HelpSGFAPIRoot);

            urlPath = urlPath.Replace("/", "");
            urlPath += "/";

            var request = new RestRequest(urlPath + "json", Method.GET);

            // execute the request
            IRestResponse response = client.Execute(request);
            var content = response.Content;

            var location = JsonConvert.DeserializeObject<Location>(content);
            return location;
            //RestResponse<Location> location = client.Execute<Location>(request);
        }
    }
}