using Microsoft.Xrm.Sdk.Query;

namespace CCLLC.Xrm.Sdk.Utilities.Search
{
    public class AndClause : SearchQuerySignatureBase
    {
        public AndClause() : base()
        {
            this.FilterOperator = LogicalOperator.And;            
        }
    }
}
