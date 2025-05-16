using KIOS.Integration.Web.Model;
using POS_IntegrationCommonDTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CreateOrderResponse = KIOS.Integration.Web.Model.CreateOrderResponse;

namespace KIOS.Integration.Web.Services.Interfaces
{
    public interface ICreateOrderDTService
    {
        Task<CreateOrderResponse> CreateOrder(CreateOrderModel request);
        Task<CreateOrderResponse> UpdateOrder(UpdateOrderModel request, string thirdPartyOrderId);
    }
}
