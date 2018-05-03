using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace CCLCC.Xrm.Utilities
{
    public class MergedEntities
    {
        private IOrganizationService OrganizationService { get; set; }

        public MergedEntities(IOrganizationService OrgService)
        {
            this.OrganizationService = OrgService;
        }

        /// <summary>
        /// Returns an entity based on the target with additional column attributres merged in from the database if a 
        /// the attribues do not aleady exist in the target entity. Columns can be specified as a ColumnSet object, a 
        /// string array, or a deliminated string. If Columns are not provided, then all available columns 
        /// will be included in the merged record.
        /// </summary>
        /// <param name="Target"></param>
        /// <param name="Columns"></param>
        /// <returns></returns>
        public Entity GetMergedRecord(Entity Target, ColumnSet Columns)
        {
            //create a new entity record and merge the target data into the new record
            Entity result = new Entity(Target.LogicalName);
            result.Id = Target.Id;
            result.MergeWith(Target);

            //merge the specified columns from the database into the new record.
            result.MergeWith(OrganizationService.Retrieve(Target.LogicalName,Target.Id, Columns));

            return result;
        }

        public Entity GetMergedRecord(Entity Target, string[] Columns)
        {
            return this.GetMergedRecord(Target, new ColumnSet(Columns));
        }

        public Entity GetMergedRecord(Entity Target, string Columns)
        {
            string[] cols = Columns.Split(new char[] { ',', ';' });
            return this.GetMergedRecord(Target, cols);
        }

        public Entity GetMergedRecord(Entity Target)
        {
            return this.GetMergedRecord(Target, new ColumnSet(true));
        }
        
        
      





    }

}

