using KIOS.Integration.Application.Queries;
using POS_Integration_CommonCore.Response;

namespace KIOS.Integration.Application.Services.Abstraction
{
    public interface ICreateOrderService
    {
        Task<ResponseModelWithClass<CreateSalesOrderResponse>> CreateOrderKFC(KIOS.Integration.Application.Commands.CreateRetailTransactionCommand request);
        Task<ResponseModelWithClass<CreateSalesOrderResponse>> CreateOrderKFCA(KIOS.Integration.Application.Commands.CreateRetailTransactionCommand request);
    }
}
