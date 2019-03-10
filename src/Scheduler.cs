using DNSUpdater.Features.GetIP;
using DNSUpdater.Features.UpdateDNS;
using DNSUpdater.Http;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DNSUpdater
{
    public interface IScheduler
    {
        Task Start(CancellationToken cancellationToken = default);
    }

    public class Scheduler : IScheduler
    {
        private const int CHECK_DELAY_DEFAULT = 30000;

        private readonly IMediator mediator;
        private readonly ILogger<Scheduler> logger;
        private readonly SchedulerSettings settings;

        public Scheduler(
            IMediator mediator,
            ILogger<Scheduler> logger,
            SchedulerSettings settings)
        {
            this.mediator = mediator;
            this.logger = logger;
            this.settings = settings;
        }

        public async Task Start(CancellationToken cancellationToken = default)
        {
            var checkDelayStr = settings.CheckDelay;
            if (!int.TryParse(checkDelayStr, out int delay))
            {
                delay = CHECK_DELAY_DEFAULT;
            }

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                try
                {
                    var currentIP = await mediator.Send(new GetIPRequest(), cancellationToken);
                    var update = await mediator.Send(new UpdateDNSRequest
                    {
                        IP = currentIP
                    }, cancellationToken);
                }
                catch (ApiException apiException)
                {
                    logger.LogError(apiException, $"The request return the status code '{apiException.StatusCode}' - '{apiException.Content}'");
                }
                catch (IPBadFormatException ipException)
                {
                    logger.LogError(ipException, "Without a valid IP address, DNS entries cannot be updated.");
                    break;
                }
                catch (System.Exception exception)
                {
                    logger.LogCritical(exception, "An unknow error has occured. For support you to visit https://github.com/dungtri/docker-cloudflare-dns-updater/issues, check if a solution to your issue exist otherwise you can report the error.");
                    break;
                }

                logger.LogInformation($"Wait {delay / 1000} seconds before the next check");
                await Task.Delay(delay, cancellationToken);
            }
        }
    }
}
