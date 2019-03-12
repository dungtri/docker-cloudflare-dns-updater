using System;

namespace DNSUpdater
{
    public interface IEnvVarReader
    {
        string Get(string key);
    }

    public class EnvVarReader : IEnvVarReader
    {
        public string Get(string key)
        {
            string value = null;

            var targets = new[] {
                EnvironmentVariableTarget.Machine,
                EnvironmentVariableTarget.User,
                EnvironmentVariableTarget.Process
            };

            foreach (var target in targets)
            {
                var targetValue = Environment.GetEnvironmentVariable(key, target);
                if (!string.IsNullOrWhiteSpace(targetValue))
                {
                    value = targetValue;
                }
            }

            return value;
        }
    }
}
