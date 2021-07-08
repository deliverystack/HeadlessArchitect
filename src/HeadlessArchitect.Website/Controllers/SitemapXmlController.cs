using Deliverystack.DeliveryApi.Models;
using Deliverystack.Models;
using HeadlessArchitect.Website.Controls;

namespace HeadlessArchitect.Website.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;

//    [Route("[controller]")]

// ControllerBase?

    public class SitemapXmlController : Controller
    {
        private PathApiClient _client;
        
        public SitemapXmlController(PathApiClient client)
        {
            _client = client;
        }

        //TODO: get from siteconfig
        //TODO: cache output until publish
        private string baseUrl = "https://localhost:44305";

            if (!entry.PageData.ExcludeFromSitemap)
            {
                double priority;

                if (entry.PageData.Priority > 0)
                {
                    priority = entry.PageData.Priority.Value;
                }
                else
                {
                    /// 1.0, .9, .8...
                    priority = 1 - (.1 * ((entry.Url.Length - entry.Url.Length.Replace("/", "")) - 1));
                }

                if (priority < 0)
                {
                    priority = 0;
                }

                ChangeFrequency changeFrequency = ChangeFrequency.Never;

                if (entry.PageData.ChangeFrequency.HasValue)
                {
                    changeFrequency = entry.PageData.ChangeFrequency.Value;
                }

                _siteMapBuilder.AddUrl(
                    baseUrl + webpageEntry.Url,
                    webpageEntry.UpdatedAt, 
                    changeFrequency,
                    priority);

                foreach (Tuple<string, string> pair in _repository.GetChildIdentifiers(webpageEntry.EntryUid))
                {
                    Recurse(_repository.Get<WebpageEntry>(pair.Item1, pair.Item2), level + 1);
                }
            }
        }

        public async Task<ActionResult> SitemapAsync()
        {
            SitemapXmlBuilder siteMapBuilder = new SitemapXmlBuilder();

            foreach (PathApiEntryModel entry in _client.Get(
                new PathApiBindingModel() {Path = "/", Descendants = 3}).Entries)
            {
                if (entry.PageData.IncludeInXmlSiteMap)
                {
                    siteMapBuilder.AddUrl(entry.Url, entry.Modified, entry.ChangeFrequency, entry.Priority);
                }
            }

            return Content(siteMapBuilder.ToString(), "text/xml");
        }
    }
}