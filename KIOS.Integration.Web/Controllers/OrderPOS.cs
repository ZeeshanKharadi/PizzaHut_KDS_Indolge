using Azure.Core;
using KIOS.Integration.Application.Commands;
using KIOS.Integration.Application.Services.Abstraction;
using KIOS.Integration.Web.Model;
using KIOS.Integration.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using POS_Integration_CommonCore.Enums;
using POS_Integration_CommonCore.Response;
using System.Net;

namespace KIOS.Integration.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderPOS : ControllerBase
    {

        private readonly IConfiguration Iconfig;
        private string cs;
        private readonly ILogger<OrderPOS> _logger;
        private readonly ICreateOrderService _createOrderService;

        public OrderPOS(IConfiguration iconfig, ILogger<OrderPOS> logger, ICreateOrderService createOrderService)
        {
            Iconfig = iconfig;
            _createOrderService = createOrderService;
            _logger = logger;
        }

        [HttpPost]
        [Route("complete-order")]
        public async Task<ResponseModelWithClass<CustomCreateOrderResponse>> CompleteOrderCHZ(KIOS.Integration.Application.Commands.CreateRetailTransactionCommand request)
        {
            ResponseModelWithClass<CustomCreateOrderResponse> response = new ResponseModelWithClass<CustomCreateOrderResponse>();

            try
            {
                return await _createOrderService.CreateOrderCHZ(request);
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.HttpStatusCode = (int)HttpStatusCode.InternalServerError;
                response.MessageType = (int)MessageType.Error;
                response.Message = "server error msg: " + ex.Message + " | Inner exception:  " + ex.InnerException;
                return response;
            }
        }


        [HttpPost]
        [Route("return-order")]
        public async Task<ResponseModelWithClass<CustomCreateOrderResponse>> ReturnPOSOrder(KIOS.Integration.Application.Commands.CreateRetailTransactionCommand request)
        {
            ResponseModelWithClass<CustomCreateOrderResponse> response = new ResponseModelWithClass<CustomCreateOrderResponse>();

            try
            {
                return await _createOrderService.ReturnPOSOrder(request);
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.HttpStatusCode = (int)HttpStatusCode.InternalServerError;
                response.MessageType = (int)MessageType.Error;
                response.Message = "server error msg: " + ex.Message + " | Inner exception:  " + ex.InnerException;
                return response;
            }
        }
    }
}
