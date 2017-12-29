using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Linq;
using System.ServiceModel;

namespace Fildela.Web.Helpers
{
    public class EndpointExtensions
    {
        internal static EndpointAddress GetRandomEndpoint()
        {
            var endpoints = RoleEnvironment.Roles["Fildela.Worker"].Instances.Select(i => i.InstanceEndpoints["FildelaService"].IPEndpoint).ToArray();
            var r = new Random(DateTime.Now.Millisecond);
            return new EndpointAddress(string.Format("net.tcp://{0}/Database", endpoints[r.Next(endpoints.Count() - 1)]));
        }
    }
}