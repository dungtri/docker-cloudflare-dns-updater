using System;

namespace DNSUpdater.Http
{
    public class ApiException : Exception
    {
        public int StatusCode { get; internal set; }
        public string Content { get; internal set; }
    }
}
