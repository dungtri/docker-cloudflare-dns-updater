using System.Collections.Generic;

namespace DNSUpdater.Entities
{
    public class DNSRecordsResponse
    {
        public IEnumerable<DNSRecord> Result { get; set; }
    }
}
