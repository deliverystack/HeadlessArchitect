
using System.Collections.Generic;

namespace HeadlessArchitect.Core.Models.ContentTypes
{
    using System.Text.Json.Serialization;

    using Deliverystack.Core.Attributes;
    using Deliverystack.Html;
    using Deliverystack.Markdown;
    using Deliverystack.StackContent.Models.Entries;

    [ContentTypeIdentifier("page")]
    public class Page : ContentstackWebPageEntryModelBase
    {
        [JsonPropertyName("firstmarkup")]
        public MarkupField FirstMarkup { get; set; }
        
        [OpenExternalsInNewTabs(true)]
        [JsonPropertyName("secondmarkup")]
        public MarkupField SecondMarkup { get; set; } 
        
        [JsonPropertyName("markdown")]
        public List<MarkdownField> Markdown { get; set; }
//        public string Markdown { get; set; }
    }
}
