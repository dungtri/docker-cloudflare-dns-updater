using System;

namespace DNSUpdater.Settings
{
    public class EnvironmentVariableRequiredException : Exception
    {
        public string[] RequiredKeys { get; }

        public EnvironmentVariableRequiredException(string[] requiredKeys)
        {
            RequiredKeys = requiredKeys;
        }
    }
}
