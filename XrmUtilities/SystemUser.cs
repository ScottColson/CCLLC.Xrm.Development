using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using CCLLC.Xrm.Sdk.Caching;

namespace CCLLC.Xrm.Sdk.Utilities
{      

    public class SystemUser
    {
        private IOrganizationService OrganizationService { get; set; }
        private IXrmCache Cache { get; set; }

        public SystemUser(IOrganizationService orgService, IXrmCache cache)
        {
            this.OrganizationService = orgService;
            this.Cache = cache;
        }


        public bool IsSystemUser(EntityReference User)
        {
            return (User.Id == GetSystemUserId().Id);
        }

        public EntityReference GetSystemUserId()
        {
            const string CACHEKEY = "CCLLC.Xrm.Utilities.SystemUser.Id";

            var id = Cache.Get(CACHEKEY) as EntityReference;

            if (id == null)
            {

                var qry = new QueryExpression
                {
                    EntityName = "systemuser",
                    ColumnSet = new ColumnSet("systemuserid"),
                    Criteria = new FilterExpression
                    {
                        FilterOperator = LogicalOperator.And,
                        Conditions =
                    {
                        new ConditionExpression
                        {
                            AttributeName = "fullname",
                            Operator = ConditionOperator.Equal,
                            Values = {"SYSTEM"}
                        }
                    }

                    }
                };

                id = OrganizationService.RetrieveMultiple(qry).Entities[0].ToEntityReference();
                Cache.Add(CACHEKEY, id, 3600);
            }

            return id;
        }

    }
}
