using KIOS.Integration.Application.Commands;
using KIOS.Integration.Application.Services;
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
    public class orderKDSController : ControllerBase
    {
        private readonly IConfiguration Iconfig;
        private string cs;
        private readonly ILogger<orderKDSController> _logger;
        private readonly ICreateOrderService _createOrderService;
        public orderKDSController(IConfiguration iconfig, ILogger<orderKDSController> logger, ICreateOrderService createOrderService)
        {
            Iconfig = iconfig;
            _createOrderService = createOrderService;
            _logger = logger;
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
