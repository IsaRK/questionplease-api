using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using questionplease_api;
using System;
using System.Linq;

[assembly: FunctionsStartup(typeof(Startup))]
namespace questionplease_api
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddFilter(level => true);
            });

            //var config = (IConfiguration)builder.Services.First(d => d.ServiceType == typeof(IConfiguration)).ImplementationInstance;

            builder.Services.AddSingleton((s) =>
            {
                //CosmosClientBuilder cosmosClientBuilder = new CosmosClientBuilder(config[Constants.CONNECTION_STRING]);
                CosmosClientBuilder cosmosClientBuilder = 
                new CosmosClientBuilder(System.Environment.GetEnvironmentVariable(Constants.CONNECTION_STRING, EnvironmentVariableTarget.Process));

                return cosmosClientBuilder.WithConnectionModeDirect()
                    .WithApplicationRegion("North Europe")
                    .WithBulkExecution(true)
                    .Build();
            });
        }
    }
}
