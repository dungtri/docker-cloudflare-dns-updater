using DNSUpdater.Settings;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DNSUpdater
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var logger = serviceProvider.GetService<ILogger<Program>>();
            logger.LogInformation("DNSUpdater started.");

            var schedule = serviceProvider.GetService<IScheduler>();
            var cancellationTokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, e) =>
            {
                logger.LogInformation("Cancel key press detected. Cancel all tasks...");
                cancellationTokenSource.Cancel();
                e.Cancel = true;
            };
            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                logger.LogInformation("Process exit detected. Cancel all tasks...");
                cancellationTokenSource.Cancel();
            };
            await schedule.Start(cancellationTokenSource.Token);
            logger.LogInformation("DNSUpdater closed.");
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddLogging(option => option
                    .AddConsole()
                    .AddDebug()
                )
                .AddMediatR()
                .AddTransient<IEnvironmentVariable, EnvironmentVariable>()
                .AddTransient<IScheduler, Scheduler>();
        }
    }
}
