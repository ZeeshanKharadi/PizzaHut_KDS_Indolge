
using MediatR;
using POS_IntegrationCommonDTO.Response;

namespace KIOS.Integration.Application.Commands
{
    public class UpdateKDSOrderCommand : IRequest<CreateKDSOrderResponse>
    {
        public string ThirdPartyOrderId { get;set; }
        public string Reason { get;set; }

    }
}
