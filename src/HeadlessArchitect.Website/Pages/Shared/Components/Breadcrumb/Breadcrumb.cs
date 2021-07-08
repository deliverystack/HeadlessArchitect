namespace HeadlessArchitect.Website.Pages.Shared.Components.Breadcrumb
{
    using Microsoft.AspNetCore.Mvc;

    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Deliverystack.DeliveryApi.Models;
    using Deliverystack.Models;
    using System;
    using System.Linq;

    public class Breadcrumb : ViewComponent
    {
        public class BreadcrumbModel
        {
            public PathApiEntryModel[] Ancestors { get; set; }

            public PathApiEntryModel Self { get; set; }

            public static BreadcrumbModel GetFromResults(PathApiResultModel data)
            {
                var result = new BreadcrumbModel();
                using var enumerator = data.Entries.GetEnumerator();
                List<PathApiEntryModel> entries = new List<PathApiEntryModel>();
                while (enumerator.MoveNext())
                    entries.Add(enumerator.Current);
                result.Ancestors = entries.ToArray().Take(entries.Count - 1).ToArray();
                result.Self = entries.Last();
                return result;
            }
        }

        private PathApiClient _client;

        public Breadcrumb(PathApiClient client)
        {
            _client = client;
        }

        // May 18 2021 - logic is synchronous and lightweight; ignore warnings about async
        #pragma warning disable CS1998
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(BreadcrumbModel.GetFromResults(
                _client.Get(new PathApiBindingModel() { Path = this.HttpContext.Request.Path, Ancestors = Int32.MaxValue })));
        }
        #pragma warning restore CS1998 // Rethrow to preserve stack details
    }
}
