using CentralizedBilling.Infrastructure;
using CentralizedBilling.Infrastructure.Model;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Description;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace CentralizedBilling.Controllers
{
    [RoutePrefix("api/CentralizedBilling")]
    public class CentralizedBillingController : ApiController
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public async Task<IHttpActionResult> GetAccounts()
        {
            ConnectToCRM connectToCRM = new ConnectToCRM();
            var accounts = connectToCRM.GetAccounts();
            return this.Ok(accounts);
        }
    }
}
