namespace HeadlessArchitect.Website.Pages.Shared.Components.TopNav
{
using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Deliverystack.DeliveryApi.Models;
using Deliverystack.Models;

public class GoogleAnalytics : ViewComponent
{
    public class GoogleAnalyticsModel
    {
        public string GoogleAnalyticsId { get; set; }

        public GoogleAnalyticsModel(string googleAnalyticsId)
        {
            GoogleAnalyticsId = googleAnalyticsId;
        }
    }

    private readonly PathApiClient _client;

    public GoogleAnalytics(PathApiClient client)
    {
        _client = client;
    }

    // May 18 2021 - logic is synchronous and lightweight; ignore warnings about async
    #pragma warning disable CS1998
    public async Task<IViewComponentResult> InvokeAsync()
    {
        foreach (PathApiEntryModel result in _client.Get(
            new()
            {
                Path = HttpContext.Request.Path, Ancestors = int.MaxValue
            }).Entries.Reverse())
        {
            if (!String.IsNullOrEmpty(result.PageData.GoogleAnalyticsId))
            {
                return View(new GoogleAnalyticsModel(result.PageData.GoogleAnalyticsId));
            }
        }
        
        return Content(String.Empty);
    }
    #pragma warning restore CS1998 // Rethrow to preserve stack details
}
}
