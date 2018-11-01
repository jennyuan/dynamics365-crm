using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel.Description; 

namespace QueryingCRMData2
{
    class Program
    {
        static void Main(string[] args)
        {
            //connecting to Organization Service
            ClientCredentials credentials = new ClientCredentials();
            credentials.Windows.ClientCredential = System.Net.CredentialCache.DefaultNetworkCredentials;
            OrganizationServiceProxy _serviceProxy = new OrganizationServiceProxy(new Uri("http://minint-f36s5eh:5555/AdventureWorksCycles/XRMServices/2011/Organization.svc"), null, credentials, null);

            _serviceProxy.EnableProxyTypes();
            //var opportunities = QueryOpportunities(_serviceProxy);
            var opportunities = FetchXMLOpportunities(_serviceProxy);

            foreach (var record in opportunities)
            {
                Opportunity opportunity = record as Opportunity;
                decimal estRevenue = 0;
                if (opportunity.EstimatedValue != null)
                    estRevenue = decimal.Floor(opportunity.EstimatedValue.Value);
                Console.WriteLine(String.Format("Topic: {0}, Est. Revenue ${1}\n", opportunity.Name, estRevenue));
            }
            Console.ReadLine();  
        }

        static DataCollection<Entity> QueryOpportunities(OrganizationServiceProxy crmSvc)
        {
            var queryExpression = new QueryExpression()
            {
                EntityName = Opportunity.EntityLogicalName,
                ColumnSet = new ColumnSet("name", "estimatedvalue")
            };

            var queryExpressionResult = crmSvc.RetrieveMultiple(queryExpression);

            return queryExpressionResult.Entities; 
        }

        static List<Entity> FetchXMLOpportunities(IOrganizationService service)
        {
            var fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='opportunity'>
    <attribute name='name' />
    <attribute name='estimatedvalue' />
    <attribute name='opportunityid' />
    <order attribute='name' descending='false' />
  </entity>
</fetch>";

            var fetchExpression = new FetchExpression(fetchXml);
            var fetchResult = service.RetrieveMultiple(fetchExpression);

            return fetchResult.Entities.ToList(); 
        }
    }

}
