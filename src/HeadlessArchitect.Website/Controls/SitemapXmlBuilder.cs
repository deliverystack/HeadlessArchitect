namespace HeadlessArchitect.Website.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    public class SitemapXmlBuilder
    {
        private readonly XNamespace _namespace = "http://www.sitemaps.org/schemas/sitemap/0.9";
        private readonly List<SitemapUrl> _urls = new();
        
        public void AddUrl(
            string url, 
            DateTime? modified = null, 
            ChangeFrequency? changeFrequency = null, 
            double? priority = null)
        {
            _urls.Add(new SitemapUrl()
            {
                Url = url,
                Modified = modified,
                ChangeFrequency = changeFrequency,
                Priority = priority,
            });
        }

        public override string ToString()
        {
            return new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XElement(_namespace + "urlset",
                        from item in _urls
                        select CreateItemElement(item)
                    )).ToString();            
        }

        private XElement CreateItemElement(SitemapUrl sitemapUrl)
        {
            XElement element = new XElement(_namespace + "url", 
                new XElement(_namespace + "loc", sitemapUrl.Url.ToLower()));

            if (sitemapUrl.Modified.HasValue)
            {
                element.Add(new XElement(_namespace + "lastmod", 
                    sitemapUrl.Modified.Value.ToString("yyyy-MM-ddTHH:mm:ss.f") + "+00:00"));
            }

            if (sitemapUrl.ChangeFrequency.HasValue)
            {
                element.Add(new XElement(_namespace + "changefreq", 
                    sitemapUrl.ChangeFrequency.Value.ToString().ToLower()));
            }

            if (sitemapUrl.Priority.HasValue)
            {
                element.Add(new XElement(_namespace + "priority", 
                    sitemapUrl.Priority.Value.ToString("N1")));
            }

            return element;
        }
    }
    
    public enum ChangeFrequency
    {
        Undefined,
        Always,
        Hourly,
        Daily,
        Weekly,
        Monthly,
        Yearly,
        Never
    }

    public class SitemapUrl
    {
        public string Url { get; set; }
        public DateTime? Modified { get; set; }
        public ChangeFrequency? ChangeFrequency { get; set; }
        public double? Priority { get; set; }
    }
}