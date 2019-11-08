using Microsoft.Xrm.Sdk.Query;

namespace CCLLC.Xrm.Sdk.Utilities.Search
{
    public abstract class SearchQuerySignatureBase : ISearchQuerySignature
    {
        public SearchQuerySignatureBase() {

            this.FilterOperator = LogicalOperator.And;
            this.RequireQuickFind = false;
        }

        public LogicalOperator FilterOperator { get; internal set; }

        public bool RequireQuickFind { get; internal set; }

        public bool Test(FilterExpression filter)
        {
            if(RequireQuickFind && !filter.IsQuickFindFilter)
            {
                return false;
            }

            return filter.FilterOperator == this.FilterOperator;
        }
    }
}
