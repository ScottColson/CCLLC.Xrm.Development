using System.Collections.Generic;

namespace CCLCC.Telemetry.Context
{
    using Implementation;

    public class OperationContext : IOperationContext
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Name { get; set; }
        public string CorrelationVector { get; set; }

        public void CopyTo(IOperationContext target)
        {
            target.Id = Id;
            target.ParentId = ParentId;
            target.CorrelationVector = CorrelationVector;
            target.Name = Name;           
        }

        public void UpdateTags(IDictionary<string, string> tags, IContextTagKeys keys)
        {
            tags.UpdateTagValue(keys.OperationId, this.Id, keys.TagSizeLimits);
            tags.UpdateTagValue(keys.OperationParentId, this.ParentId, keys.TagSizeLimits);
            tags.UpdateTagValue(keys.OperationCorrelationVector, this.CorrelationVector, keys.TagSizeLimits);
            tags.UpdateTagValue(keys.OperationName, this.Name, keys.TagSizeLimits);
        }
    }
}
