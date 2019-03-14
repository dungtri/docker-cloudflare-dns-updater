namespace DNSUpdater.Features
{
    public class UpdateDNSSettings
    {
        public static readonly string DNS_UPDATER_EMAIL = "DNS_UPDATER_EMAIL";
        public static readonly string DNS_UPDATER_KEY = "DNS_UPDATER_KEY";
        public static readonly string DNS_UPDATER_KEY_FILE = "DNS_UPDATER_KEY_FILE";
        public static readonly string DNS_UPDATER_ZONE = "DNS_UPDATER_ZONE";
        public static readonly string DNS_UPDATER_ZONE_FILE = "DNS_UPDATER_ZONE_FILE";

        public string Email { get; set; }
        public string Key { get; set; }
        public string KeyFile { get; set; }
        public string Zone { get; set; }
        public string ZoneFile { get; set; }
    }
}
