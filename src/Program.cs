using DNSUpdater.Extensions;
using DNSUpdater.Features;
using DNSUpdater.Features.UpdateDNS;
using FluentValidation;
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
            logger.LogInformation("dns-updater started.");

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

            ValidateSettings(serviceProvider, logger);

            await schedule.Start(cancellationTokenSource.Token);
            logger.LogInformation("dns-updater closed.");
        }

        private static void ValidateSettings(ServiceProvider serviceProvider, ILogger<Program> logger)
        {
            var updateDNSSettings = serviceProvider.GetRequiredService<UpdateDNSSettings>();
            var updateDNSValidator = serviceProvider.GetRequiredService<IValidator<UpdateDNSSettings>>();
            var updateDNSValidation = updateDNSValidator.Validate(updateDNSSettings);
            if (!updateDNSValidation.IsValid)
            {
                foreach (var error in updateDNSValidation.Errors)
                {
                    logger.LogError(error.ErrorMessage);
                }

                logger.LogInformation("All environment variables marked as required has to be provide.");
                logger.LogInformation("For more information, type dns-updater --help");
            }
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddLogging(option => option
                    .AddConsole()
                    .AddDebug()
                )
                .AddMediatR()
                .AddEnvironmentSettings()
                .AddTransient<IScheduler, Scheduler>()
                .AddFluentValidation();
        }
    }
}
