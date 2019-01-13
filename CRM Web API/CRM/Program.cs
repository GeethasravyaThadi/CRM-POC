using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System.ServiceModel.Description;
using System.Net;
using System.Runtime.Serialization;
using Microsoft.Xrm.Sdk.Query;

namespace CRM
{
    class Program
    {
        static IOrganizationService _service;
        static void Main(string[] args)
        {
            ConnectToMSCRM("geeta.thadi@homeinsteadinc.com", "Ucmo.1234", "https://crmdv.homeinsteadinc.com/XRMServices/2011/Organization.svc");
           
            string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='account'>
                                    <attribute name='name' />
                                    <attribute name='primarycontactid' />
                                    <attribute name='telephone1' />
                                    <attribute name='accountid' />
                                    <order attribute='name' descending='false' />
                                  </entity>
                                </fetch>";
            var accounts = _service.RetrieveMultiple(new FetchExpression(fetchXml));
            // Guid userid = ((WhoAmIResponse)_service.Execute(new WhoAmIRequest())).UserId;
            //if (userid != Guid.Empty)
            //{
            //    Console.WriteLine("Connection Established Successfully");
            //    Console.ReadKey();
            //}
        }
        public static void ConnectToMSCRM(string UserName, string Password, string SoapOrgServiceUri)
        {
            try
            {
                ClientCredentials credentials = new ClientCredentials();
                credentials.UserName.UserName = UserName;
                credentials.UserName.Password = Password;
                Uri serviceUri = new Uri(SoapOrgServiceUri);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                OrganizationServiceProxy proxy = new OrganizationServiceProxy(serviceUri, null, credentials, null);
                proxy.EnableProxyTypes();
                _service = (IOrganizationService)proxy;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while connecting to CRM " + ex.Message);
                Console.ReadKey();
            }
        }
    }

}
