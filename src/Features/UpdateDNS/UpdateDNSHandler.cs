using DNSUpdater.Entities;
using DNSUpdater.Features.UpdateDNS;
using DNSUpdater.Http;
using DNSUpdater.Settings;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DNSUpdater
{
    public class UpdateDNSHandler : IRequestHandler<UpdateDNSRequest, UpdateDNSResponse>
    {
        public static readonly string baseUrl = "https://api.cloudflare.com/client/v4/";
        private readonly IEnvironmentVariable environmentVariable;
        private readonly ILogger<UpdateDNSHandler> _logger;

        public UpdateDNSHandler(IEnvironmentVariable environmentVariable, ILogger<UpdateDNSHandler> logger)
        {
            this.environmentVariable = environmentVariable;
            _logger = logger;
        }

        public async Task<UpdateDNSResponse> Handle(UpdateDNSRequest request, CancellationToken cancellationToken)
        {
            var variables = environmentVariable.GetRequired
            (
                EnvironmentVariables.DNS_UPDATER_EMAIL,
                EnvironmentVariables.DNS_UPDATER_KEY,
                EnvironmentVariables.DNS_UPDATER_ZONE
            );

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-Auth-Email", new[] {
                    variables[EnvironmentVariables.DNS_UPDATER_EMAIL]
                });
                client.DefaultRequestHeaders.Add("X-Auth-Key", new[] {
                    variables[EnvironmentVariables.DNS_UPDATER_KEY]
                });

                var zone = variables[EnvironmentVariables.DNS_UPDATER_ZONE];
                var url = $"{baseUrl}/zones/{zone}/dns_records?type=A";
                using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, url))
                using (var response = await client.SendAsync(httpRequest, cancellationToken))
                {
                    var stream = await response.Content.ReadAsStreamAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var data = DeserializeJsonFromStream<DNSRecordsResponse>(stream);
                        await Update(client, data.Result, request.IP, zone, cancellationToken);
                        return new UpdateDNSResponse();
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    throw new ApiException
                    {
                        StatusCode = (int)response.StatusCode,
                        Content = content
                    };
                }
            }
        }

        private static T DeserializeJsonFromStream<T>(Stream stream)
        {
            if (stream == null || stream.CanRead == false)
            {
                return default;
            }

            using (var stremReader = new StreamReader(stream))
            using (var textReader = new JsonTextReader(stremReader))
            {
                var serializer = new JsonSerializer()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
                var searchResult = serializer.Deserialize<T>(textReader);
                return searchResult;
            }
        }

        private async Task Update(HttpClient client, IEnumerable<DNSRecord> records, string ip, string zone, CancellationToken cancellationToken)
        {
            foreach (var record in records)
            {
                if (record.Content != ip)
                {
                    var url = $"{baseUrl}/zones/{zone}/dns_records/{record.Id}";
                    var json = JsonConvert.SerializeObject(new DNSRecord()
                    {
                        Type = record.Type,
                        Name = record.Name,
                        Content = ip,
                        Proxied = record.Proxied,
                        TTL = record.TTL,
                        Proxiable = record.Proxiable
                    }, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    });

                    using (var request = new HttpRequestMessage(HttpMethod.Put, url)
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json")
                    })
                    using (var response = await client.SendAsync(request, cancellationToken))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            throw new ApiException
                            {
                                StatusCode = (int)response.StatusCode,
                                Content = content
                            };
                        }
                    }
                }
            }
        }
    }
}
