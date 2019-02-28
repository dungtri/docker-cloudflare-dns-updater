using DNSUpdater.Http;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace DNSUpdater.Features.GetIP
{
    public class GetIPHandler : IRequestHandler<GetIPRequest, string>
    {
        private const string IPServiceUrl = "http://ipv4bot.whatismyipaddress.com";
        private const string IPRegEx = @"^(?=.*[^\.]$)((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.?){4}$";
        private readonly ILogger<GetIPHandler> logger;

        public GetIPHandler(ILogger<GetIPHandler> logger)
        {
            this.logger = logger;
        }

        private bool ValidateIP(string value)
        {
            var rx = new Regex(IPRegEx, RegexOptions.Compiled);
            return rx.IsMatch(value);
        }

        public async Task<string> Handle(GetIPRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation($"Get IP from {IPServiceUrl}...");

            using (HttpClient client = new HttpClient())
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, IPServiceUrl))
            using (var response = await client.SendAsync(httpRequest, cancellationToken))
            {
                var content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new ApiException()
                    {
                        StatusCode = (int)response.StatusCode,
                        Content = content
                    };
                }

                if (!ValidateIP(content))
                {
                    throw new IPBadFormatException($"Public IP is not valid: '{content}'");
                }

                logger.LogInformation($"Public IP: {content}");
                return content;
            }
        }
    }
}
