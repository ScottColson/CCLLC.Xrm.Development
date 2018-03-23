using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.XrmPluginExtensions.Utilities
{
    using Caching;
    using Microsoft.Xrm.Sdk.Query;

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
            const string CACHEKEY = "CCLCC.XrmPluginExtensions.Utilities.SystemUser.Id";

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
