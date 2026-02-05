using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using Wolverine;
using Wolverine.RabbitMQ;

namespace Common
{
    public static class WolwerineExtensions
    {
        public static async Task UseWolverineRabbitMqAsync(this IHostApplicationBuilder builder 
            , Action<WolverineOptions> configure)
        {
            var retryPolicy = Policy.Handle<BrokerUnreachableException>()
                .Or<ConnectFailureException>()
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception ,timeSpan, retryCount ) =>
                {
                    Console.WriteLine($"Wolverine RabbitMQ connection failed. Waiting {timeSpan} before next retry. Retry attempt {retryCount}");
                });

            await retryPolicy.ExecuteAsync(async () =>
            {
                var connString = builder.Configuration.GetConnectionString("messaging") ??
                 throw new InvalidOperationException("RabbitMQ connection string 'messaging' not found.");

                var factory = new ConnectionFactory()
                {
                    Uri = new Uri(connString)
                };

               await using var connection =await factory.CreateConnectionAsync();
              
            });

            builder.UseWolverine(conf =>
            {
                conf.UseRabbitMqUsingNamedConnection("messaging").AutoProvision()
                .DeclareExchange("questions");
                configure(conf);
            });

            builder.Services.AddOpenTelemetry().WithTracing(conf =>
            {
                conf.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(builder.Environment.ApplicationName))
                .AddSource("Wolverine");
            });
        }
    }
}
