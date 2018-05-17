using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace CCLCC.Xrm.Sdk.Utilities
{
    public class MetaData
    {
        private IOrganizationService OrganizationService { get; set; }

        public MetaData(IOrganizationService orgService)
        {
            this.OrganizationService = orgService;
        }
        /// <summary>
        /// Returns a list of strings that represent each metadata attribute
        /// for the specified entity schema name that can be used in a create operation. 
        /// </summary>
        /// <param name="EntityName"></param>
        /// <returns></returns>
        public List<string> GetMetaDataCreateFieldNames(string entityLogicalName)
        {
            List<string> fieldNameList = new List<string>();

            AttributeMetadata[] attributeMetadataArray = this.RetrieveAttributeMetaData(entityLogicalName);

            foreach (AttributeMetadata item in attributeMetadataArray)
            {
                if ((bool)item.IsValidForCreate)
                {
                    fieldNameList.Add(item.LogicalName);
                }
            }

            return fieldNameList;
        }

        /// <summary>
        /// Returns a list of strings that represent each metadata attribute
        /// for the specified entity schema name that can be used in an update operation. 
        /// </summary>
        /// <param name="EntityName"></param>
        /// <returns></returns>
        public List<string> GetMetaDataUpdateFieldNames(string entityLogicalName)
        {
            List<string> fieldNameList = new List<string>();

            AttributeMetadata[] attributeMetadataArray = this.RetrieveAttributeMetaData(entityLogicalName);

            foreach (AttributeMetadata item in attributeMetadataArray)
            {
                if ((bool)item.IsValidForUpdate)
                {
                    fieldNameList.Add(item.LogicalName);
                }
            }

            return fieldNameList;
        }


        /// <summary>
        /// Returns the text associated with specified optionset value for the identified entity and attribute.
        /// </summary>
        /// <param name="entityLogicalName">Schema name of the entity that contains the optionset attribute.</param>
        /// <param name="attributeName">Schema name of the attribute.</param>
        /// <param name="selectedValue">Numeric value of the optionset.</param>
        /// <returns></returns>
        public string GetOptionSetLabel(string entityLogicalName, string attributeName, int optionSetValue)
        {
            string returnLabel = string.Empty;

            OptionMetadataCollection optionsSetLabels = null;

            optionsSetLabels = this.RetrieveOptionSetMetaDataCollection(entityLogicalName, attributeName);

            foreach (OptionMetadata optionMetdaData in optionsSetLabels)
            {
                if (optionMetdaData.Value == optionSetValue)
                {
                    returnLabel = optionMetdaData.Label.UserLocalizedLabel.Label;
                    break;
                }
            }

            return returnLabel;
        }

        /// <summary>
        /// Returns an array of AttributeMetadata for the specified entity name.
        /// </summary>
        /// <param name="entityLogicalName"></param>
        /// <returns></returns>
        public AttributeMetadata[] RetrieveAttributeMetaData(string entityLogicalName)
        {
            RetrieveEntityRequest retrieveEntityRequest = new RetrieveEntityRequest();
            RetrieveEntityResponse retrieveEntityResponse = new RetrieveEntityResponse();

            retrieveEntityRequest.LogicalName = entityLogicalName;
            retrieveEntityRequest.EntityFilters = Microsoft.Xrm.Sdk.Metadata.EntityFilters.Attributes;

            retrieveEntityResponse = (RetrieveEntityResponse)OrganizationService.Execute(retrieveEntityRequest);

            return retrieveEntityResponse.EntityMetadata.Attributes;
        }

        /// <summary>
        /// Returns the OptionMetadataCollection for the specified entity and attribute.
        /// </summary>
        /// <param name="entityLogicalName"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public OptionMetadataCollection RetrieveOptionSetMetaDataCollection(string entityLogicalName, string attributeName)
        {
            OptionMetadataCollection returnOptionsCollection = null;
            RetrieveEntityRequest retrieveEntityRequest = new RetrieveEntityRequest();
            RetrieveEntityResponse retrieveEntityResponse = new RetrieveEntityResponse();

            retrieveEntityRequest.LogicalName = entityLogicalName;
            retrieveEntityRequest.EntityFilters = Microsoft.Xrm.Sdk.Metadata.EntityFilters.Attributes;

            retrieveEntityResponse = (RetrieveEntityResponse)OrganizationService.Execute(retrieveEntityRequest);

            AttributeMetadata[] attributeMetadataArray = this.RetrieveAttributeMetaData(entityLogicalName);

            foreach (AttributeMetadata attributeMetadata in attributeMetadataArray)
            {
                if (attributeMetadata.AttributeType == AttributeTypeCode.Picklist &&
                    attributeMetadata.LogicalName == attributeName)
                {
                    returnOptionsCollection = ((PicklistAttributeMetadata)attributeMetadata).OptionSet.Options;
                    break;
                }
                else if (attributeMetadata.AttributeType == AttributeTypeCode.Status &&
                    attributeMetadata.LogicalName == attributeName)
                {
                    returnOptionsCollection = ((StatusAttributeMetadata)attributeMetadata).OptionSet.Options;
                    break;
                }
                else if (attributeMetadata.AttributeType == AttributeTypeCode.State &&
                    attributeMetadata.LogicalName == attributeName)
                {
                    returnOptionsCollection = ((StateAttributeMetadata)attributeMetadata).OptionSet.Options;
                    break;
                }
            }

            return returnOptionsCollection;
        }
    }

}
