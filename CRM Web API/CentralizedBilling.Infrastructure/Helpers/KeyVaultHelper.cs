using CentralizedBilling.Infrastructure.Model;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace CentralizedBilling.Infrastructure.Helpers
{
    public class KeyVaultHelper
    {
        KeyVaultSettings keyVaultSettings = new KeyVaultSettings() { ClientId = ConfigurationManager.AppSettings["ApplicationId"], ClientSecret = ConfigurationManager.AppSettings["ApplicationSecret"], VaultUrl = ConfigurationManager.AppSettings["KeyVaultUrl"] };

        //public KeyVaultHelper(IOptions<KeyVaultSettings> keyVaultSettings)
        //{
        //    this.keyVaultSettings = keyVaultSettings.Value;
        //}

        /// <summary>
        /// Returns the Key for the requested Secret
        /// </summary>
        /// <param name="secretToGet">Name of the secret</param>
        /// <returns>String for the resource</returns>
        /// AuthenticationCallback is Delegate async method
        
        public async Task<string> GetStorageKey(string secretToGetKey)
        {
            var client = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessTokenAsync), new HttpClient());
            var secret = client.GetSecretAsync(keyVaultSettings.VaultUrl, secretToGetKey).GetAwaiter().GetResult();

            return secret.Value;
        }

        /* AuthenticationCallback is a delegate function and the value of authority/resource/scope is provide by SDK, 
        we need to provide the delegate function to use these values to get the access token.*/

        private async Task<string> GetAccessTokenAsync(string authority, string resource, string scope)
        {
            var clientCredential = new ClientCredential(keyVaultSettings.ClientId, keyVaultSettings.ClientSecret);
            var context = new AuthenticationContext(authority, TokenCache.DefaultShared);
            var result = await context.AcquireTokenAsync(resource, clientCredential);

            return result.AccessToken;
        }
    }
}
