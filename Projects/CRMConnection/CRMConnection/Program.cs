using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;
using System.ServiceModel.Description; 

namespace CRMConnection
{
    class Program
    {
        static void Main(string[] args)
        {
            ClientCredentials credentials = new ClientCredentials();
            credentials.Windows.ClientCredential = System.Net.CredentialCache.DefaultNetworkCredentials;
            OrganizationServiceProxy _serviceProxy = new OrganizationServiceProxy(new Uri("http://minint-f36s5eh:5555/AdventureWorksCycles/XRMServices/2011/Organization.svc"), null, credentials, null);

            //Entity newAccount = BuildAccount();
            //Guid createdRecordId = _serviceProxy.Create(newAccount);
            //Console.WriteLine(String.Format("Account GUID: {0}", createdRecordId)); 

            _serviceProxy.EnableProxyTypes();
            Contact newContact = new Contact();
            newContact.FirstName = "Alan";
            newContact.LastName = "Jackson";
            newContact.Telephone1 = "555-9999";
            Guid createdContactId = _serviceProxy.Create(newContact);
            Console.WriteLine(String.Format("Contact GUID: {0}", createdContactId)); 
            
            WhoAmIRequest whoAmI = new WhoAmIRequest();
            WhoAmIResponse userLoggedId = (WhoAmIResponse)_serviceProxy.Execute(whoAmI);

            if (userLoggedId != null)
            {
                Console.WriteLine(String.Format("Organization ID: {0}\nBusiness Unit ID: {1}\nuser ID:{2}", userLoggedId.OrganizationId, userLoggedId.BusinessUnitId, userLoggedId.UserId));
                Console.ReadLine();
            } 
        }

        static Entity BuildAccount()
        {
            Entity newAccountRecord = new Entity("account");
            newAccountRecord["name"] = "Microsoft";
            newAccountRecord["address1_line1"] = "One Microsoft Way";
            return newAccountRecord;
        } 
    }
}
