using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D365.XrmPluginExtensions.Utilities
{
    public class RolesAccessAndTeams
    {
        private IOrganizationService OrganizationService { get; set; }

        public RolesAccessAndTeams(IOrganizationService OrgService)
        {
            this.OrganizationService = OrgService;
        }


        /// <summary>
        /// Grants a specific user Read Access to a specific Entity record.
        /// </summary>
        /// <param name="OrganizationService"></param>
        /// <param name="Record">EntityReference of the record.</param>
        /// <param name="UserId">GUID of user that is getting access.</param>
        public void GrantUserReadAccess(EntityReference Record, Guid UserId)
        {

            PrincipalAccess principalAccess = new PrincipalAccess();
            principalAccess.Principal = new EntityReference("systemuser", UserId);
            principalAccess.AccessMask = AccessRights.ReadAccess;

            GrantAccessRequest grantAccessRequest = new GrantAccessRequest();
            grantAccessRequest.Target = Record;
            grantAccessRequest.PrincipalAccess = principalAccess;

            GrantAccessResponse grantAccessResponse = (GrantAccessResponse)OrganizationService.Execute(grantAccessRequest);
        }

        public void GrantReadAccess(EntityReference Record, EntityReference Principal)
        {

            PrincipalAccess principalAccess = new PrincipalAccess();
            principalAccess.Principal = Principal;
            principalAccess.AccessMask = AccessRights.ReadAccess;

            GrantAccessRequest grantAccessRequest = new GrantAccessRequest();
            grantAccessRequest.Target = Record;
            grantAccessRequest.PrincipalAccess = principalAccess;

            GrantAccessResponse grantAccessResponse = (GrantAccessResponse)OrganizationService.Execute(grantAccessRequest);
        }

        public void GrantAccess(EntityReference Record, EntityReference Principal, AccessRights AccessRights)
        {
            PrincipalAccess principalAccess = new PrincipalAccess();
            principalAccess.Principal = Principal;
            principalAccess.AccessMask = AccessRights;

            GrantAccessRequest grantAccessRequest = new GrantAccessRequest();
            grantAccessRequest.Target = Record;
            grantAccessRequest.PrincipalAccess = principalAccess;

            GrantAccessResponse grantAccessResponse = (GrantAccessResponse)OrganizationService.Execute(grantAccessRequest);
        }

        public AccessRights GetAccessRights(EntityReference Record, EntityReference Principal)
        {
            RetrievePrincipalAccessRequest req = new RetrievePrincipalAccessRequest
            {
                Principal = Principal,
                Target = Record
            };

            RetrievePrincipalAccessResponse resp = (RetrievePrincipalAccessResponse)OrganizationService.Execute(req);

            return resp.AccessRights;
        }

        /// <summary>
        /// Returns true if the Principal has write access to the specified record.
        /// </summary>
        /// <param name="Record"></param>
        /// <param name="Principal"></param>
        /// <returns></returns>
        public bool HasWriteAccess(EntityReference Record, EntityReference Principal)
        {
            AccessRights rights = this.GetAccessRights(Record, Principal);

            return (rights & AccessRights.WriteAccess) != AccessRights.None;
        }

        /// <summary>
        /// Evaluates users role assignments and returns true if the user is assigned one or more of the role names provided.
        /// </summary>
        /// <param name="UserId">GUID of the user.</param>
        /// <param name="RoleNames">Name of the roles supplied in a string array.</param>
        /// <returns></returns>
        public bool IsAssignedRole(Guid UserId, string[] RoleNames, bool includeTeamMembership = true)
        {
            //if array is empty then return false
            if (RoleNames == null || RoleNames.Length <= 0)
            {
                return false;
            }

            //check for wildcard and return true if it exists
            foreach (string r in RoleNames)
            {
                if (r == "*")
                {
                    return true;
                }
            }

            //Check for assignment of role directly to the user by querying the role table with
            //inner join to the systemuserrole table.
            var qry = new QueryExpression
            {
                EntityName = "role",
                ColumnSet = new ColumnSet(new string[] { "name" }),

                LinkEntities = {
                    new LinkEntity
                    {
                        JoinOperator = JoinOperator.Inner,
                        LinkFromEntityName = "role",
                        LinkFromAttributeName = "roleid",
                        LinkToEntityName = "systemuserroles",
                        LinkToAttributeName = "roleid",

                        LinkCriteria = new FilterExpression
                        {
                            FilterOperator = LogicalOperator.And,
                            Conditions =
                            {
                                new ConditionExpression("systemuserid",ConditionOperator.Equal,UserId)
                            }
                        }
                    }
                }
            };

            //Add role filter with OR clause for one or more matching names. 
            FilterExpression roleFilter = new FilterExpression
            {
                IsQuickFindFilter = false,
                FilterOperator = LogicalOperator.Or,
            };

            foreach (string name in RoleNames)
            {
                roleFilter.Conditions.Add(new ConditionExpression("name", ConditionOperator.Equal, name));
            }

            //set the qry filter equal to the role filter
            qry.Criteria = roleFilter;

            var result = OrganizationService.RetrieveMultiple(qry);

            //if roles is assigned to the user directly we are done.
            if (result.Entities.Count > 0)
            {
                return true;
            }


            //If required, check for assignment of role to user via team membershi pby querying the role table with
            //inner join to the teamrole table where the user is a member of the team.
            if (includeTeamMembership)
            {
                qry = new QueryExpression
                {
                    EntityName = "role",
                    ColumnSet = new ColumnSet(new string[] { "name" }),

                    LinkEntities =
                    {
                        new LinkEntity //INNER JOIN TeamRoles
                        {
                            JoinOperator = JoinOperator.Inner,
                            LinkFromEntityName = "role",
                            LinkFromAttributeName = "roleid",
                            LinkToEntityName = "teamroles",
                            LinkToAttributeName = "roleid",

                            LinkEntities =
                            {
                                new LinkEntity //INNER JOIN TeamMembership where user is a team member
                                {
                                    JoinOperator = JoinOperator.Inner,
                                    LinkFromEntityName = "teamroles",
                                    LinkFromAttributeName = "teamid",
                                    LinkToEntityName = "teammembership",
                                    LinkToAttributeName = "teamid",
                                    LinkCriteria = new FilterExpression
                                    {
                                        FilterOperator = LogicalOperator.Or,
                                        Conditions =
                                        {
                                            new ConditionExpression("systemuserid",ConditionOperator.Equal,UserId)
                                        }
                                    }
                                }
                            }
                        }
                    }
                };

                //Add role filter with OR clause for one or more matching names. 
                roleFilter = new FilterExpression
                {
                    IsQuickFindFilter = false,
                    FilterOperator = LogicalOperator.Or,
                };

                foreach (string name in RoleNames)
                {
                    roleFilter.Conditions.Add(new ConditionExpression("name", ConditionOperator.Equal, name));
                }

                //set the qry filter equal to the role filter
                qry.Criteria = roleFilter;

                result = OrganizationService.RetrieveMultiple(qry);

                if (result.Entities.Count() > 0)
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Evaluates users role assignments and returns true if the user is assigned one or more of the role names provided.
        /// </summary>
        /// <param name="UserId">GUID of the user.</param>
        /// <param name="RoleName">Name of the role</param>
        /// <returns></returns>
        public bool IsAssignedRole(Guid UserId, string RoleName, bool includeTeamMembership = true)
        {
            if (string.IsNullOrEmpty(RoleName))
            {
                return false;
            }

            //convert the single role name to an array and call the overloaded function.
            string[] roleNames = new string[] { RoleName };
            return this.IsAssignedRole(UserId, roleNames, includeTeamMembership);
        }


        /// <summary>
        /// Evaluates a users membership in a team and returns true if the user is a team member.
        /// </summary>
        /// <param name="SystemUser">EntityReference of the user.</param>
        /// <param name="Team">EntityReference of the team.</param>
        /// <returns></returns>
        public bool IsTeamMember(EntityReference SystemUser, EntityReference Team)
        {
            if (SystemUser == null || SystemUser.LogicalName != "systemuser")
            {
                throw new Exception("SystemUser parameter must represent a CRM SystemUser record.");
            }

            if (Team == null || Team.LogicalName != "team")
            {
                throw new Exception("Team parameter must represent a team.");
            }

            QueryByAttribute qry = new QueryByAttribute("teammembership");
            qry.AddAttributeValue("teamid", Team.Id);
            qry.AddAttributeValue("systemuserid", SystemUser.Id);
            qry.ColumnSet = new ColumnSet(new string[] { "teammembershipid" });

            EntityCollection results = OrganizationService.RetrieveMultiple(qry);
            if (results.Entities.Count > 0)
            {
                return true;
            }

            return false;
        }


        public bool IsTeamMember(EntityReference SystemUser, string Team)
        {
            if (SystemUser == null || SystemUser.LogicalName != "systemuser")
            {
                throw new Exception("SystemUser parameter must represent a CRM SystemUser record.");
            }

            if (string.IsNullOrEmpty(Team))
            {
                throw new Exception("Team parameter must not be null.");
            }

            QueryExpression qry = new QueryExpression()
            {
                EntityName = "teammembership",
                Criteria =
                {
                    FilterOperator = LogicalOperator.And,
                    Conditions = {
                        new ConditionExpression("systemuserid",ConditionOperator.Equal, SystemUser.Id)
                    }
                },
                LinkEntities = {
                    new LinkEntity {
                        JoinOperator = Microsoft.Xrm.Sdk.Query.JoinOperator.Inner,
                        LinkFromEntityName = "teammembership",
                        LinkFromAttributeName = "teamid",
                        LinkToEntityName = "team",
                        LinkToAttributeName = "teamid",
                        LinkCriteria = {
                            FilterOperator  = LogicalOperator.And,
                            Conditions = {
                                new ConditionExpression("name",ConditionOperator.Equal,Team)
                            }
                        }
                    }
                }
            };

            EntityCollection results = OrganizationService.RetrieveMultiple(qry);
            if (results.Entities.Count > 0)
            {
                return true;
            }

            return false;

        }


    }
}
