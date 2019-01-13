using System;
using System.Collections.Generic;
using System.Text;

namespace CentralizedBilling.Infrastructure.Model
{
	/// <summary>
	/// 
	/// </summary>
   public class KeyVaultSettings
    {
		/// <summary>
		/// 
		/// </summary>
        public string VaultUrl { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string ClientId { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string ClientSecret { get; set; }
    }
}
