using System;
using System.Collections.Generic;
using System.Linq;

namespace DNSUpdater.Settings
{
    public interface IEnvironmentVariable
    {
        string Get(string key);
        string GetRequired(string key);
        IDictionary<string, string> GetRequired(params string[] keys);
    }

    public class EnvironmentVariable : IEnvironmentVariable
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

        public string GetRequired(string key)
        {
            string value = Get(key);
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new EnvironmentVariableRequiredException(new[] {key});
            }
            return value;
        }

        public IDictionary<string, string> GetRequired(params string[] keys)
        {
            var variables = keys
                .Select(key => new KeyValuePair<string, string>(key, Get(key)))
                .ToDictionary(variable => variable.Key, variable => variable.Value);

            var requiredKeys = variables
                .Where(variable => string.IsNullOrWhiteSpace(variable.Value))
                .Select(variable => variable.Key)
                .ToArray();

            if (requiredKeys.Any())
            {
                throw new EnvironmentVariableRequiredException(requiredKeys);
            }
            return variables;
        }
    }
}
