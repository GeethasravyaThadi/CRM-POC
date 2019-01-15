using CentralizedBilling.Infrastructure.Helpers;
using CentralizedBilling.Infrastructure.Model;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Description;
using System.Text;

namespace CentralizedBilling.Infrastructure
{
    public class ConnectToCRM
    {
        private readonly KeyVaultHelper keyVaultHelper=new KeyVaultHelper();

        
        static IOrganizationService _service;
        public EntityCollection GetAccounts()
        {

            ConnectToMSCRM(this.keyVaultHelper.GetStorageKey("crmserviceaccount").GetAwaiter().GetResult(), this.keyVaultHelper.GetStorageKey("crmserviceaccountpassword").GetAwaiter().GetResult(), "https://crmdv.homeinsteadinc.com/XRMServices/2011/Organization.svc");
            string fetchXml = FetchXMLBuilder.GetAccountXML();
            var accounts = _service.RetrieveMultiple(new FetchExpression(fetchXml));
            return accounts;
        }

        public void ConnectToMSCRM(string UserName, string Password, string SoapOrgServiceUri)
        {
            try
            {
                ClientCredentials credentials = new ClientCredentials();
                credentials.UserName.UserName = UserName;
                credentials.UserName.Password = Password;
                Uri serviceUri = new Uri(SoapOrgServiceUri);
               // ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                OrganizationServiceProxy proxy = new OrganizationServiceProxy(serviceUri, null, credentials, null);
                proxy.EnableProxyTypes();
                _service = (IOrganizationService)proxy;
            }
            catch (Exception ex)
            {

            }
        }


        // Get's the authorization header from ADFS.
        //Please Ignore this code right now. we used this code for ADFS OAuth2 . Right now we are performing Authorization based on Azure Key Vault
        public static string GetAuthorizationHeader()
        {

            string authority = "https://crmadfsdv.homeinsteadinc.com/adfs/oauth2/";
            string resourceURI = "https://crmdv.homeinsteadinc.com/api/data/v8.2/";
            string clientID = "189D848C-1037-43BA-A9CF-C415400DD7E4";
            string clientReturnURI = "http://localhost:54762/Default";

            try
            {
                AuthenticationContext ac = new AuthenticationContext(authority, false);
                AuthenticationResult ar = ac.AcquireToken(resourceURI, clientID, new Uri(clientReturnURI));
                string authHeader = ar.CreateAuthorizationHeader();

                return authHeader;
            }
            catch (Exception ex)
            {
                string message = ex.Message;

                if (ex.InnerException != null)
                {
                    message += "InnerException : " + ex.InnerException.Message;
                }

                //  Label1.Text = message;
            }

            return null;
        }

        // Gets the Account Information from CRM Dev using OAuth2.
        //Please Ignore this code right now. we used this code for ADFS OAuth2 . Right now we are performing Authorization based on Azure Key Vault
        public static JObject GetAccountsInfomationFromCRM(string authorizationCode)
        {
            JObject accountJson = null;

            if (authorizationCode == null)
            {
                return null;
            }
            try
            {
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://crmdv.homeinsteadinc.com/api/data/v8.2/accounts");
                request.Headers.TryAddWithoutValidation("Authorization", authorizationCode);

                HttpResponseMessage response = client.SendAsync(request).GetAwaiter().GetResult();

                string responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                accountJson = JObject.Parse(responseString);
                //string serializedContent = JsonConvert.SerializeObject(responseString);

            }
            catch (WebException ex)
            {
                //  Label1.Text = ex.Message;
            }
            catch (Exception ex)
            {
                // Label1.Text = ex.Message;

            }
            return accountJson;
        }


    }
}
