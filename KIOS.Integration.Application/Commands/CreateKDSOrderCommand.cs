using MediatR;
using POS_IntegrationCommonDTO.Response;

namespace KIOS.Integration.Application.Commands
{
    public class CreateKDSOrderCommand : IRequest<CreateKDSOrderResponse>
    {
        public string ThirdPartyOrderId { get; set; }
        public string StoreId { get; set; }
        
        public IList<CreatePrintSalesOrderTransCCRequest> CreateKDSLineCommand { get; set; }

        public CreateKDSOrderCommand()
        {
            CreateKDSLineCommand = new List<CreatePrintSalesOrderTransCCRequest>();
        }
    }
}
