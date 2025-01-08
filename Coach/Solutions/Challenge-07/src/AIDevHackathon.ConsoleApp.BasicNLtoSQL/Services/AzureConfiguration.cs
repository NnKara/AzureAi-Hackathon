using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIDevHackathon.ConsoleApp.BasicNLtoSQL
{
    // This class is used to read the configuration from the appsettings.json file
    internal class AzureConfiguration
    {
        private IConfiguration _config = null;
        public AzureConfiguration()
        {
            var isDevelopment = string.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), "development", StringComparison.InvariantCultureIgnoreCase);

            if (isDevelopment)
            {
                _config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.Development.json", true, true)
                    .Build();
            }
            else
            {
                _config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", true, true)
                    .Build();
            }

            AOAIEndpoint = _config["Azure:AOAIEndpoint"];
            AOAIKey = _config["Azure:AOAIKey"];
            AOAIDeploymentId = _config["Azure:AOAIDeploymentId"];
        }
        public string AOAIEndpoint { get; set; }
        public string AOAIKey { get; set; }
        public string AOAIDeploymentId { get; set; }
    }
}
