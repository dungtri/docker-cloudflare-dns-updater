using DNSUpdater.Features;
using Microsoft.Extensions.DependencyInjection;

namespace DNSUpdater.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFluentValidation(this IServiceCollection services)
        {
            var validators = FluentValidation.AssemblyScanner.FindValidatorsInAssemblies(new[] { System.Reflection.Assembly.GetExecutingAssembly() });
            foreach (var validator in validators)
            {
                services.AddTransient(validator.InterfaceType, validator.ValidatorType);
            }

            return services;
        }

        public static IServiceCollection AddEnvironmentSettings(this IServiceCollection services)
        {
            return services
                .AddTransient<IEnvVarReader, EnvVarReader>()
                .AddSingleton((serviceProvider) =>
                {
                    var reader = serviceProvider.GetService<IEnvVarReader>();
                    return new UpdateDNSSettings
                    {
                        Email = reader.Get(UpdateDNSSettings.DNS_UPDATER_EMAIL),
                        Key = reader.Get(UpdateDNSSettings.DNS_UPDATER_KEY),
                        KeyFile = reader.Get(UpdateDNSSettings.DNS_UPDATER_KEY_FILE),
                        Zone = reader.Get(UpdateDNSSettings.DNS_UPDATER_ZONE),
                        ZoneFile = reader.Get(UpdateDNSSettings.DNS_UPDATER_ZONE_FILE)
                    };
                })
                .AddSingleton((serviceProvider) =>
                {
                    var reader = serviceProvider.GetService<IEnvVarReader>();
                    return new SchedulerSettings
                    {
                        CheckDelay = reader.Get(SchedulerSettings.SCHEDULER_CHECK_DELAY)
                    };
                });
        }
    }
}
