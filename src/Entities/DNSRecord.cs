using Newtonsoft.Json;
using System;

namespace DNSUpdater.Entities
{
    public class DNSRecord
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public bool? Proxiable { get; set; }
        public bool? Proxied { get; set; }
        public int? TTL { get; set; }
        public bool? Locked { get; set; }
        [JsonProperty("zone_id")] public string ZoneId { get; set; }
        [JsonProperty("zone_name")] public string ZoneName { get; set; }
        [JsonProperty("modified_on")] public DateTime? ModifiedOn { get; set; }
        [JsonProperty("created_on")] public DateTime? CreatedOn { get; set; }
    }
}
