using KIOS.Integration.Application.Commands;
using KIOS.Integration.Application.Queries;
using POS_Integration_CommonCore.Response;

namespace KIOS.Integration.Application.Services.Abstraction
{
    public interface ICreateOrderService
    {
        Task<ResponseModelWithClass<CustomCreateOrderResponse>> CreateOrderCHZ(KIOS.Integration.Application.Commands.CreateRetailTransactionCommand request);
        Task<ResponseModelWithClass<CustomCreateOrderResponse>> ReturnPOSOrder(KIOS.Integration.Application.Commands.CreateRetailTransactionCommand request);
     }
}
