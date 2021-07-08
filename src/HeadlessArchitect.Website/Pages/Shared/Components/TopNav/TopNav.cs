namespace HeadlessArchitect.Website.Pages.Shared.Components.TopNav
{
    using Microsoft.AspNetCore.Mvc;

    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Deliverystack.DeliveryApi.Models;
    using Deliverystack.Models;

    public class TopNav : ViewComponent
    {
        public class TopNavModel
        {
            public PathApiEntryModel Home { get; private set; }

            public PathApiEntryModel[] Children { get; private set; }

            public static TopNavModel GetFromResults(PathApiResultModel data)
            {
                var result = new TopNavModel();

                using var enumerator = data.Entries.GetEnumerator();
                if (enumerator.MoveNext()) result.Home = enumerator.Current;
                var children = new List<PathApiEntryModel>();
                while (enumerator.MoveNext())
                    children.Add(enumerator.Current);
                result.Children = children.ToArray();
                return result;
            }
        }

        private readonly PathApiClient _client;

        public TopNav(PathApiClient client)
        {
            _client = client;
        }

        // May 18 2021 - logic is synchronous and lightweight; ignore warnings about async
        #pragma warning disable CS1998
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(TopNavModel.GetFromResults(
                _client.Get(new() { Path = "/", Descendants = 1 })));
        }
        #pragma warning restore CS1998 // Rethrow to preserve stack details
    }
}
