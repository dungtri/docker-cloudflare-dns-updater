using DNSUpdater.Features.GetIP;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DNSUpdater.IntegrationTests
{
    public class GetIPHandlerTests
    {
        [Fact]
        public async Task Should_ReturnIP()
        {
            var logger = Substitute.For<ILogger<GetIPHandler>>();
            var handler = new GetIPHandler(logger);
            var ip = await handler.Handle(new GetIPRequest(), CancellationToken.None);

            Assert.NotNull(ip);
        }
    }
}
