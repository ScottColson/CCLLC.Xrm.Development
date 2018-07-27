using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;

namespace CCLLC.Xrm.Sdk.Utilities
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
        public void AssociateRecords(EntityReference TargetRecord, EntityReferenceCollection RelatedRecords, Relationship Relationship)
        {
            try
            {                
                OrganizationService.Associate(TargetRecord.LogicalName, TargetRecord.Id, Relationship, RelatedRecords);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error Associating Records: ", ex.Message));
            }
        }

        public void AssociateRecords(EntityReference TargetRecord, EntityReference RelatedRecord, Relationship Relationship)
        {
            EntityReferenceCollection relatedRecords = new EntityReferenceCollection();
            relatedRecords.Add(RelatedRecord);
            this.AssociateRecords(TargetRecord, relatedRecords, Relationship);
        }

        /// <summary>
        /// Returns a list of records of the specified type that are related to the target record
        /// via the specified relationshp.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="TargetRecord"></param>
        /// <param name="Relationship"></param>
        /// <param name="EntityLogicalName"></param>
        /// <param name="columms"></param>
        /// <returns></returns>
        public IList<T> GetAssociatedRecords<T>(EntityReference TargetRecord, Relationship Relationship, string EntityLogicalName, ColumnSet columms = null) where T : Entity
        {
            try
            {
                if (TargetRecord == null) throw new ArgumentNullException("TargetRecord is required.");
                if (Relationship == null) throw new ArgumentNullException("Relationship is required.");
                if (string.IsNullOrEmpty(EntityLogicalName)) { throw new ArgumentNullException("EntityLogicalName is required."); }

                if (columms == null)
                {
                    columms = new ColumnSet(EntityLogicalName + "id");
                }

                var associatedRecords = new List<T>();
                
                var qry = new QueryExpression
                {
                    EntityName = EntityLogicalName,
                    ColumnSet = columms
                };

                var qryCollection = new RelationshipQueryCollection();
                qryCollection.Add(Relationship, qry);

                var req = new Microsoft.Xrm.Sdk.Messages.RetrieveRequest();
                req.RelatedEntitiesQuery = qryCollection;
                req.Target = TargetRecord;
                req.ColumnSet = new ColumnSet(TargetRecord.LogicalName + "id");

                var resp = (RetrieveResponse)OrganizationService.Execute(req);

                if (resp.Entity.RelatedEntities.Contains(Relationship))
                {
                    foreach (var record in resp.Entity.RelatedEntities[Relationship].Entities)
                    {                        
                        associatedRecords.Add((T)record);
                    }
                }

                return associatedRecords;
               
            }
            catch(Exception ex)
            {
                throw new Exception(string.Format("Error in GetAssociatedRecords: {0}", ex.Message), ex);
            }

        }

        

    }
}
