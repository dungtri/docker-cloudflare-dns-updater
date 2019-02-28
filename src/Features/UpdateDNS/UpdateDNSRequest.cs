using MediatR;

namespace DNSUpdater.Features.UpdateDNS
{
    public class UpdateDNSRequest : IRequest<UpdateDNSResponse>
    {
        public string IP { get; set; }
    }
}
