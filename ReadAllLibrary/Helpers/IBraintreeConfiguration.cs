using Braintree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReadAllLibrary.Helpers
{
    /// <summary>
    /// IBraintreeConfiguration Interface
    /// Holds method signatures which may be used to implement BraintreeConfiguration.
    /// </summary>
    public interface IBraintreeConfiguration
    {
        IBraintreeGateway CreateGateway();
        string GetConfigurationSetting(string setting);
        Braintree.Environment GetEnvironment();
        IBraintreeGateway GetGateway();
    }
}