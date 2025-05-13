using KIOS.Integration.Application.Commands;
using KIOS.Integration.Application.Services;
using KIOS.Integration.Web.Services;
using KIOS.Integration.Application.Services.Abstraction;
using KIOS.Integration.Web.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using POS_Integration_CommonCore.Enums;
using POS_Integration_CommonCore.Response;
using POS_IntegrationCommonDTO.Response;
using System.Net;
using CreateOrderResponse = KIOS.Integration.Web.Model.CreateOrderResponse;

namespace KIOS.Integration.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class orderKDSController : ControllerBase
    {
        private readonly IConfiguration Iconfig;
        private string cs;
        private readonly ILogger<orderKDSController> _logger;
        private readonly ICreateOrderService _createOrderService;
        private readonly ICreateOrderDTService _createOrderDTService;
        public orderKDSController(IConfiguration iconfig, ILogger<orderKDSController> logger, ICreateOrderService createOrderService, ICreateOrderDTService createOrderDTService)
        {
            Iconfig = iconfig;
            _createOrderService = createOrderService;
            _logger = logger;
            _createOrderDTService = createOrderDTService;
        }

        [HttpPost]
        [Route("CreateKDSOrder")]
        public async Task<ResponseModelWithClass> CreateIndolgeOrders(OrderHeader request)
        {
            ResponseModelWithClass res = new ResponseModelWithClass();

            cs = Iconfig.GetConnectionString("CSKDS");

            IndolgeOrdersToKDS obj = new IndolgeOrdersToKDS(Iconfig);
            ResponseModelWithClass response = await obj.InsertIndolgeOrdersToKDS(request);
           
            return response;
        }

        [HttpPut]
        [Route("UpdateKDSOrder")]
        public async Task<ResponseModelWithClass> UpdateIndolgeOrders(string OrderId, List<Orders> request)
        {
            ResponseModelWithClass res = new ResponseModelWithClass();

            cs = Iconfig.GetConnectionString("CSKDS");

            IndolgeOrdersToKDS obj = new IndolgeOrdersToKDS(Iconfig);
            ResponseModelWithClass response = await obj.UpdateIndolgeOrdersToKDS(OrderId, request);
            //if ( res.HttpStatusCode == (int)HttpStatusCode.OK && res.MessageType == (int)MessageType.Success )
            //{

            //}
            return response;
        }

        [HttpDelete]
        [Route("cancelKDSOrder")]
        public async Task<ResponseModelWithClass> Delete(string OrderId)
        {
            ResponseModelWithClass res = new ResponseModelWithClass();
            cs = Iconfig.GetConnectionString("CSKDS");

            IndolgeOrdersToKDS obj = new IndolgeOrdersToKDS(Iconfig);
            ResponseModelWithClass response = await obj.DeleteOrderFromKDS(cs,OrderId);

            return response;
        }

        [HttpPost]
        [Route("createOrderForDragonTail")]
        public async Task<CreateOrderResponse> CreateOrder(CreateOrderModel request)
        {
            CreateOrderResponse response = new CreateOrderResponse();

            try
            {
                return await _createOrderDTService.CreateOrder(request);
            }
            catch (Exception ex)
            {
                response.Result = null;
                // response.HttpStatusCode = (int)HttpStatusCode.InternalServerError;
                response.HttpStatusCode = 0;
                //response.MessageType = (int)MessageType.Error;
                response.MessageType = 0;
                response.Message = "server error msg: " + ex.Message + " | Inner exception:  " + ex.InnerException;
                return response;
            }
        }

        [HttpPut]
        [Route("updateOrder")]
        public async Task<CreateOrderResponse> UpdateOrder(string thirdPartyOrderId, [FromBody] UpdateOrderModel request)
        {
            CreateOrderResponse response = new CreateOrderResponse();

            try
            {

                return await _createOrderPOSService.UpdateOrder(request, thirdPartyOrderId);
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.HttpStatusCode = 0; // Consider using actual status codes
                response.MessageType = 0;
                response.Message = $"server error msg: {ex.Message} | Inner exception: {ex.InnerException}";
                return response;
            }
        }

        //[HttpPost]
        //[Route("complete-order")]
        //public async Task<ResponseModelWithClass<CustomCreateOrderResponse>> CompleteOrderCHZ(KIOS.Integration.Application.Commands.CreateRetailTransactionCommand request)
        //{
        //    ResponseModelWithClass<CustomCreateOrderResponse> response = new ResponseModelWithClass<CustomCreateOrderResponse>();

        //    try
        //    {
        //        return await _createOrderService.CreateOrderCHZ(request);
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Result = null;
        //        response.HttpStatusCode = (int)HttpStatusCode.InternalServerError;
        //        response.MessageType = (int)MessageType.Error;
        //        response.Message = "server error msg: " + ex.Message + " | Inner exception:  " + ex.InnerException;
        //        return response;
        //    }
        //}

    }
}
