using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Braintree;
using System.Configuration;

/// <summary>
/// Namespace which holds helper methods required throughout the application.
/// </summary>
namespace ReadAllLibrary.Helpers
{
    /// <summary>
    /// BraintreeConfiguartion class, Implements Interface IBraintreeConfigiration
    /// Contains the implementation of various methods relating the to creation of braintree payments
    /// </summary>
    public class BraintreeConfiguration : IBraintreeConfiguration
    {
        //Properties
        public Braintree.Environment Environment { get; set; }
        public string MerchantId { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        private IBraintreeGateway BraintreeGateway { get; set; }

        /// <summary>
        /// Method which uses the required braintree Keys to create a payment gateway
        /// </summary>
        /// <returns>Braintree Gateway</returns>
        public IBraintreeGateway CreateGateway()
        {
            Environment = GetEnvironment();
            MerchantId = GetConfigurationSetting("BraintreeMerchantId");
            PublicKey = GetConfigurationSetting("BraintreePublicKey");
            PrivateKey = GetConfigurationSetting("BraintreePrivateKey");

            return new BraintreeGateway
            {
                Environment = Environment,
                MerchantId = MerchantId,
                PublicKey = PublicKey,
                PrivateKey = PrivateKey
            };
        }

        /// <summary>
        /// Method which retrives the braintree configuration setting dependent on the setting value which is unput
        /// </summary>
        /// <param name="setting">Parameter to be configured</param>
        /// <returns>Configured settings</returns>
        public string GetConfigurationSetting(string setting)
        {
            return ConfigurationManager.AppSettings[setting];
        }

        /// <summary>
        /// Method which returns the braintree sandbox environment
        /// </summary>
        /// <returns>Environment dependant on settings</returns>
        public Braintree.Environment GetEnvironment()
        {
            string environment = GetConfigurationSetting("BraintreeEnvironment");
            return environment == "production" ? Braintree.Environment.PRODUCTION : Braintree.Environment.SANDBOX;
        }

        /// <summary>
        /// Getter method for braintree gateway
        /// Creates a new Gateway if BrainTreeGateWay is null
        /// </summary>
        /// <returns>BrainTreeGateway</returns>
        public IBraintreeGateway GetGateway()
        {
            if (BraintreeGateway == null)
            {
                BraintreeGateway = CreateGateway();
            }

            return BraintreeGateway;
        }
    }
}