using System.Collections.Generic;
using HelpSGFWebApp.Models;
using HelpSGFWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HelpSGFWebApp.Pages
{
    public class Locations : PageModel
    {
        private DataService dataService = new DataService();

        [ViewData]
        public List<LocationSearchResultItem> LocationList { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Search { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string Category { get; set; }

        public void OnGet()
        {
            if (!string.IsNullOrEmpty(Search))
            {
                LocationList = dataService.SearchLocations(Search);
            }
            
            if (!string.IsNullOrEmpty(Category))
            {
                LocationList = dataService.FilterLocations(Category, 0);
            }
        }
    }
}