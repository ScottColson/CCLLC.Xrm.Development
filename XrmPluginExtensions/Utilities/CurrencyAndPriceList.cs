using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D365.XrmPluginExtensions.Utilities
{
    public class CurrencyAndPriceList
    {
        private IOrganizationService OrganizationService { get; set; }

        public CurrencyAndPriceList(IOrganizationService orgService)
        {
            this.OrganizationService = orgService;
        }

        public EntityReference GetPriceListByName(string priceListName)
        {
            EntityReference rv = null;

            QueryByAttribute qry = new QueryByAttribute("pricelevel");
            qry.ColumnSet = new ColumnSet(new string[] { "pricelevelid" });
            qry.AddAttributeValue("name", priceListName);
            qry.AddAttributeValue("statecode", 0);

            EntityCollection results = OrganizationService.RetrieveMultiple(qry);
            if (results.Entities.Count == 1)
            {
                rv = results.Entities[0].ToEntityReference();
            }

            return rv;
        }


        /// <summary>
        /// Returns a reference to the CRM Currency record identified by the specified currency code.
        /// </summary>
        /// <param name="OrganizationService"></param>
        /// <param name="currencyCode"></param>
        /// <returns></returns>
        public EntityReference GetCurrencyByCode(string currencyCode)
        {
            EntityReference rv = null;

            QueryByAttribute qry = new QueryByAttribute("transactioncurrency");
            qry.ColumnSet = new ColumnSet(new string[] { "transactioncurrencyid" });
            qry.AddAttributeValue("isocurrencycode", currencyCode);
            qry.AddAttributeValue("statecode", 0);

            EntityCollection results = OrganizationService.RetrieveMultiple(qry);
            if (results.Entities.Count == 1)
            {
                rv = results.Entities[0].ToEntityReference();
            }

            return rv;
        }

        /// <summary>
        /// Returns a reference to the CRM Currency record identified by USD currency code.
        /// </summary>
        /// <param name="OrganizationService"></param>
        /// <returns></returns>
        public EntityReference GetCurrencyUSD()
        {
            return this.GetCurrencyByCode("USD");
        }

    }
}
