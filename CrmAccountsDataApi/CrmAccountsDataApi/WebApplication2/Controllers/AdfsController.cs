using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace CrmAccountsDataApi.Controllers
{
    [RoutePrefix("api/CentralizedBilling")]
    public class AdfsController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public async Task<IHttpActionResult> GetAccounts()
        {
            JObject accounts=new JObject() ;
          
            string clientID = "189D848C-1037-43BA-A9CF-C415400DD7E4";
            string redirectUri = "http://localhost:54762/Default";
           

            try
            {
                var postParams = new Dictionary<string, string> {
                     { "username", @"hisc\_oauth2 _dvcrm16" },
                     { "password", "Use service account Password here" },
                };
                // make a POST request to the "cards" endpoint and pass in the parameters
                CookieCollection cc= MakeRequestCookie<dynamic>("POST", postParams);
             string authCode= GetAuthurizationCode<dynamic>("POST", cc, postParams);

                var postParamsAcq = new Dictionary<string, string> {
                    { "grant_type", "authorization_code" },
                    { "client_id", clientID },
                    { "redirect_uri", redirectUri },
                     { "code", authCode }
                };

                var token = GetAquireToken<dynamic>("POST",postParamsAcq);

                accounts = GetCRMData("https://crmdv.homeinsteadinc.com/api/data/v8.2/accounts", token);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return this.Ok(accounts);
        }

        /// <summary>
        /// To get the cookie information from the HttpResponseMessage after HttpClinet call
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="httpMethod">POST</param>
        /// <param name="postParams">Required parameters</param>
        /// <returns></returns>
        private static CookieCollection MakeRequestCookie<T>(string httpMethod, Dictionary<string, string> postParams = null)
        {
            var cookieContainer = new CookieContainer();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            httpClientHandler.CookieContainer = cookieContainer;
            httpClientHandler.AllowAutoRedirect = false;
            
            using (var client = new HttpClient(httpClientHandler))
            {
                var client1 = new HttpClient(httpClientHandler);
                HttpRequestMessage requestMessage = new HttpRequestMessage(new HttpMethod(httpMethod), $"https://crmadfsdv.homeinsteadinc.com/adfs/oauth2/authorize?response_type=code&client_id=189D848C-1037-43BA-A9CF-C415400DD7E4&resource=https://crmdv.homeinsteadinc.com/api/data/v8.2/&redirect_uri=http://localhost:54762/Default");
                
                if (postParams != null)
                    requestMessage.Content = new FormUrlEncodedContent(postParams);   // This is where your content gets added to the request body

                HttpResponseMessage response = client.SendAsync(requestMessage).Result;

                var cookie = cookieContainer.GetCookies(new Uri("https://crmadfsdv.homeinsteadinc.com/adfs/oauth2/authorize?response_type=code&client_id=189D848C-1037-43BA-A9CF-C415400DD7E4&resource=https://crmdv.homeinsteadinc.com/api/data/v8.2/&redirect_uri=http://localhost:54762/Default"));
                 try
                {
                    // Attempt to deserialise the reponse to the desired type, otherwise throw an expetion with the response from the api.
                    if (cookie.Count != 0)
                        return cookie;
                    else
                        throw new Exception();
                }
                catch (Exception ex)
                {
                    throw new Exception($"An error ocurred while calling the API. It responded with the following message: {response.StatusCode} {response.ReasonPhrase}");
                }
            }
        }
        /// <summary>
        /// Get the Auturization code by sending the cookie information to HTTP POST call
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="httpMethod"><POST/param>
        /// <param name="cc">Cookie Collection</param>
        /// <param name="postParams">Required parameters</param>
        /// <returns></returns>
        private static string GetAuthurizationCode<T>(string httpMethod, CookieCollection cc, Dictionary<string, string> postParams = null)
        {
            var cookieContainer = new CookieContainer();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            httpClientHandler.CookieContainer = cookieContainer;
            httpClientHandler.AllowAutoRedirect = false;

            using (var client = new HttpClient(httpClientHandler))
            {
                var client1 = new HttpClient(httpClientHandler);
                httpClientHandler.CookieContainer.Add(cc);
                HttpRequestMessage requestMessage = new HttpRequestMessage(new HttpMethod(httpMethod), $"https://crmadfsdv.homeinsteadinc.com/adfs/oauth2/authorize?response_type=code&client_id=189D848C-1037-43BA-A9CF-C415400DD7E4&resource=https://crmdv.homeinsteadinc.com/api/data/v8.2/&redirect_uri=http://localhost:54762/Default");

                if (postParams != null)
                    requestMessage.Content = new FormUrlEncodedContent(postParams);   // This is where your content gets added to the request body



                HttpResponseMessage response = client.SendAsync(requestMessage).Result;
                
                Uri defaultUri = response.Headers.Location;
                var code = defaultUri.Query;

                string querystring = code.Substring(code.IndexOf('?'));
                string[] arr = querystring.Split('=');
               
                try
                {
                    // Attempt to deserialise the reponse to the desired type, otherwise throw an expetion with the response from the api.
                    if (arr.Length>0)
                        return arr[1].ToString();
                    else
                        throw new Exception();
                }
                catch (Exception ex)
                {
                    throw new Exception($"An error ocurred while calling the API. It responded with the following message: {response.StatusCode} {response.ReasonPhrase}");
                }
            }
        }
        /// <summary>
        /// To get the Bearer Token by sending authcode and other parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="httpMethod">POST</param>
        /// <param name="postParams">Required Parameters</param>
        /// <returns></returns>
        private static string GetAquireToken<T>(string httpMethod, Dictionary<string, string> postParams = null)
        {
            var cookieContainer = new CookieContainer();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            httpClientHandler.CookieContainer = cookieContainer;
            httpClientHandler.AllowAutoRedirect = false;

            using (var client = new HttpClient(httpClientHandler))
            {
                var client1 = new HttpClient(httpClientHandler);
                HttpRequestMessage requestMessage = new HttpRequestMessage(new HttpMethod(httpMethod), $"https://crmadfsdv.homeinsteadinc.com/adfs/oauth2/token");

                if (postParams != null)
                    requestMessage.Content = new FormUrlEncodedContent(postParams);   // This is where your content gets added to the request body

                HttpResponseMessage response = client.SendAsync(requestMessage).Result;

                try
                {
                    string responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                  dynamic acuireToken=JsonConvert.DeserializeObject(responseString);
                    string acuire_Token = acuireToken["access_token"];
                    // Attempt to deserialise the reponse to the desired type, otherwise throw an expetion with the response from the api.
                    return acuire_Token;
                }
                catch (Exception ex)
                {
                    throw new Exception($"An error ocurred while calling the API. It responded with the following message: {response.StatusCode} {response.ReasonPhrase}");
                }
            }
        }
        
        /// <summary>
        /// To get the CRM entity data by  using bearer token.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private static JObject GetCRMData(string url,string token)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request =
                new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.TryAddWithoutValidation("Authorization", token);
            HttpResponseMessage response = client.SendAsync(request).GetAwaiter().GetResult();
            string responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            dynamic returnJson = JsonConvert.DeserializeObject(responseString);
            return returnJson;
        }
    }
}
