using Microsoft.Xrm.Sdk.Query;

namespace CCLLC.Xrm.Sdk.Utilities.Search
{
    public class OrClause : SearchQuerySignatureBase
    {
        public OrClause() : base()
        {
            this.FilterOperator = LogicalOperator.Or;            
        }
    }
}
