namespace HeadlessArchitect.Website.Pages.Shared.Components.Breadcrumb
{
using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Deliverystack.DeliveryApi.Models;
using Deliverystack.Models;

public class FavIcon : ViewComponent
{
    public class FavIconModel
    {
        public string Path { get; }

        public FavIconModel(PathApiResultModel data)
        {
            foreach (var entry in data.Entries.Reverse())
            {
                if (!string.IsNullOrEmpty(entry.PageData?.FavIcon))
                {
                    Path = entry.PageData.FavIcon;
                    break;
                }
            }
        }
    }
    
    private PathApiClient _client;

    public FavIcon(PathApiClient client)
    {
        _client = client;
    }

    // May 18 2021 - logic is synchronous and lightweight; ignore warnings about async
    #pragma warning disable CS1998
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var favIconModel = new FavIconModel(
            _client.Get(new PathApiBindingModel() 
                { Path = this.HttpContext.Request.Path, Ancestors = Int32.MaxValue }));

        if (String.IsNullOrEmpty(favIconModel.Path))
        {
            return Content(String.Empty);
        }

        return View(favIconModel);
    }
    #pragma warning restore CS1998 // Rethrow to preserve stack details
}
}
