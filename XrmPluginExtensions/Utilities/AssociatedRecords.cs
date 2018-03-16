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
    public class AssociatedRecords
    {
        private IOrganizationService OrganizationService { get; set; }

        public AssociatedRecords(IOrganizationService OrgService)
        {
            this.OrganizationService = OrgService;
        }


        /// <summary>
        /// Associate a target record and a collection of records based on the named relationshp.
        /// </summary>
        /// <param name="TargetRecord"></param>
        /// <param name="RelatedRecords"></param>
        /// <param name="RelationshipName"></param>
        public void AssociateRecords(EntityReference TargetRecord, EntityReferenceCollection RelatedRecords, string RelationshipName)
        {
            try
            {
                Relationship relationship = new Relationship(RelationshipName);
                OrganizationService.Associate(TargetRecord.LogicalName, TargetRecord.Id, relationship, RelatedRecords);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error Associating Records: ", ex.Message));
            }
        }

        public void AssociateRecords(EntityReference TargetRecord, EntityReference RelatedRecord, string RelationshipName)
        {
            EntityReferenceCollection relatedRecords = new EntityReferenceCollection();
            relatedRecords.Add(RelatedRecord);
            this.AssociateRecords(TargetRecord, relatedRecords, RelationshipName);
        }


        public List<EntityReference> GetAssociatedRecords(EntityReference TargetRecord, Relationship Relationship, string RelatedEntityLogicalName)
        {
            List<EntityReference> relatedItems = new List<EntityReference>();

            QueryExpression qry = new QueryExpression();
            qry.EntityName = RelatedEntityLogicalName;
            qry.ColumnSet = new ColumnSet(new string[] { RelatedEntityLogicalName + "id" });

            RelationshipQueryCollection relationshipQueryCollection = new RelationshipQueryCollection();
            relationshipQueryCollection.Add(Relationship, qry);

            RetrieveRequest retrieveRequest = new RetrieveRequest();
            retrieveRequest.RelatedEntitiesQuery = relationshipQueryCollection;
            retrieveRequest.ColumnSet = new ColumnSet(new string[] { TargetRecord.LogicalName + "id" });
            retrieveRequest.Target = TargetRecord;

            RetrieveResponse retrieveResponse = (RetrieveResponse)OrganizationService.Execute(retrieveRequest);

            if (retrieveResponse.Entity.RelatedEntities.Contains(Relationship))
            {
                foreach (Entity item in retrieveResponse.Entity.RelatedEntities[Relationship].Entities)
                {
                    relatedItems.Add(item.ToEntityReference());
                }
            }

            return relatedItems;

        }

        

    }
}
