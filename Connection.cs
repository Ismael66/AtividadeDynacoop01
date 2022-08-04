using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System.Configuration;

namespace Dynacoop
{
    public static class Connection
    {
        private static IOrganizationService _service;
 
        public static IOrganizationService Obter()
        {
            if (_service == null)
            {
                var connectionString = ConfigurationManager.ConnectionStrings["Dev"];
                if (connectionString != null)
                {
                    var crmServiceClient = new CrmServiceClient(connectionString.ConnectionString);
                    if (!string.IsNullOrEmpty(crmServiceClient.LastCrmError))
                    {
                        if (crmServiceClient.LastCrmException != null)
                        {
                            throw crmServiceClient.LastCrmException;
                        }
                    }
                    _service = crmServiceClient.OrganizationWebProxyClient;
                }
            }
            return _service;
        }
    }
}