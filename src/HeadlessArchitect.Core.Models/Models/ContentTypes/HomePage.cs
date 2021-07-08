namespace HeadlessArchitect.Core.Models.ContentTypes
{
    using Deliverystack.Core.Attributes;
    using Deliverystack.StackContent.Models.Entries;
    using Deliverystack.StackContent.Models.Fields;
    using System.Collections.Generic;

    [ContentTypeIdentifier("homepage")]
    public class HomePage : ContentstackWebPageEntryModelBase
    {
        public List<ReferenceField> Reference { get; set; }
    }
}
