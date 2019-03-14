using FluentValidation;
using System.IO;

namespace DNSUpdater.Features.UpdateDNS
{
    public class UpdateDNSSettingsValidator : AbstractValidator<UpdateDNSSettings>
    {
        public UpdateDNSSettingsValidator()
        {
            string keyRequiredMessage = $"{UpdateDNSSettings.DNS_UPDATER_KEY} or {UpdateDNSSettings.DNS_UPDATER_KEY_FILE} should be specify as environment variable.";
            When(updateSettings => string.IsNullOrEmpty(updateSettings.KeyFile), () =>
            {
                RuleFor(updateSettings => updateSettings.Key)
                    .Must(key => !string.IsNullOrWhiteSpace(key))
                    .WithMessage(keyRequiredMessage);
            })
            .Otherwise(() => {
                RuleFor(updateSettings => updateSettings.KeyFile)
                    .Must(keyFile => !string.IsNullOrWhiteSpace(keyFile))
                    .WithMessage(keyRequiredMessage)
                    .Must(keyFile => File.Exists(keyFile))
                    .WithMessage(updateSettings => $"{updateSettings.KeyFile} specify on {UpdateDNSSettings.DNS_UPDATER_KEY_FILE} not found.");
            });

            string zoneRequiredMessage = $"{UpdateDNSSettings.DNS_UPDATER_ZONE} or {UpdateDNSSettings.DNS_UPDATER_ZONE_FILE} should be specify as environment variable.";
            When(updateSettings => string.IsNullOrEmpty(updateSettings.ZoneFile), () =>
            {
                RuleFor(updateSettings => updateSettings.Zone)
                    .Must(zone => !string.IsNullOrWhiteSpace(zone))
                    .WithMessage(zoneRequiredMessage);
            })
            .Otherwise(() =>
            {
                RuleFor(updateSettings => updateSettings.ZoneFile)
                    .Must(zoneFile => !string.IsNullOrWhiteSpace(zoneFile))
                    .WithMessage(zoneRequiredMessage)
                    .Must(zoneFile => File.Exists(zoneFile))
                    .WithMessage(updateSettings => $"{updateSettings.ZoneFile} specify on {UpdateDNSSettings.DNS_UPDATER_ZONE_FILE} not found.");
            });
        }
    }
}
