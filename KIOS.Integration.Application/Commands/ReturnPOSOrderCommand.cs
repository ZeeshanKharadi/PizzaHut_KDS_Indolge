
using MediatR;
using POS_IntegrationCommonDTO.Response;

namespace KIOS.Integration.Application.Commands
{
    public class ReturnPOSOrderCommand 
    {
        public string thirdPartyOrderId { get; set; }

    }
}
