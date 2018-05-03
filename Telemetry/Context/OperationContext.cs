using System.Collections.Generic;
using CCLCC.Telemetry.Interfaces;

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
            Tags.CopyTagValue(this.Id, target.Id);
            Tags.CopyTagValue(this.ParentId, target.ParentId);
            Tags.CopyTagValue(this.CorrelationVector, target.CorrelationVector);
            Tags.CopyTagValue(this.Name, target.Name);           
        }

        public void UpdateTags(IDictionary<string, string> tags)
        {
            tags.UpdateTagValue(ContextTagKeys.Keys.OperationId, this.Id);
            tags.UpdateTagValue(ContextTagKeys.Keys.OperationParentId, this.ParentId);
            tags.UpdateTagValue(ContextTagKeys.Keys.OperationCorrelationVector, this.CorrelationVector);
            tags.UpdateTagValue(ContextTagKeys.Keys.OperationName, this.Name);     
        }
    }
}
