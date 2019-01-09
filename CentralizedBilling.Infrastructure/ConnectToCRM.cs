using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel.Description;
using System.Text;

namespace CentralizedBilling.Infrastructure
{
   public static class ConnectToCRM
    {
        static IOrganizationService _service;
        public static EntityCollection GetAccounts()
        {
            ConnectToMSCRM("geeta.thadi@homeinsteadinc.com", "Ucmo.1234", "https://crmdv.homeinsteadinc.com/XRMServices/2011/Organization.svc");
            string fetchXml =FetchXMLBuilder.GetAccountXML();
            var accounts = _service.RetrieveMultiple(new FetchExpression(fetchXml));
            return accounts;
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
                
            }
        }
    }
}
