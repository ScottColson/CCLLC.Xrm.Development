using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;


namespace CCLLC.Xrm.Sdk.Utilities.Search
{
    public interface IAliasedSearchQueryGenerator<TParent,TAlias> where TParent : Entity where TAlias : Entity
    {
        IAliasedSearchQueryGenerator<TParent, TAlias> WithLinkingAttribute(string attributeName);

        IAliasedSearchQueryGenerator<TParent, TAlias> WithMappedSearchFields(IDictionary<string, string> mappedFields);

        IAliasedSearchQueryGenerator<TParent, TAlias> AddMappedSearchField(string parentFieldName, string aliasFieldName);

        IAliasedSearchQueryGenerator<TParent, TAlias> AddSearchSignature(ISearchQuerySignature signature);

        QueryExpression Build();        

    }
}
