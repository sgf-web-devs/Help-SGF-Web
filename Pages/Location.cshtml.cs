using HelpSGFWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HelpSGFWebApp.Pages
{
    public class Location : PageModel
    {
        private DataService dataService = new DataService();
        
        public string LocationSlug { get; set; }

        [ViewData]
        public HelpSGFWebApp.Models.Location TheLocation { get; set; }
        
        public void OnGet(string locationSlug)
        {
            LocationSlug = locationSlug;
            TheLocation = dataService.GetLocation(locationSlug);
        }
    }
}