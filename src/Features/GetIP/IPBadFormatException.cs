using System;

namespace DNSUpdater.Features.GetIP
{
    public class IPBadFormatException : Exception
    {
        public IPBadFormatException(string message) : base(message)
        {
        }
    }
}
