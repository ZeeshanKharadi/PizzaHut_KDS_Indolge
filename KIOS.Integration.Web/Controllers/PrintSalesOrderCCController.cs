using MediatR;
using Microsoft.AspNetCore.Mvc;
using KIOS.Integration.Application.Commands;
using KIOS.Integration.Application.Queries;
using System.Net;
using POS_Integration_CommonCore.Response;
using POS_IntegrationCommonInfrastructure.Model;
using POS_Integration_CommonCore.Enums;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;
using KIOS.Integration.Application.Services.Abstraction;
using Azure;
using KIOS.Integration.Application.Services;
using KIOS.Integration.Web.Model;
using KIOS.Integration.Web.Services;

namespace KIOS.Integration.Web.Controllers
{
    //[Authorize]
    [Route("api/saleOrderCallCenter")]
    [ApiController]
    public class PrintSalesOrderCCController : ControllerBase
    {
        private readonly ISender _mediator;
        private readonly IConfiguration Iconfig;
        private readonly ICreateOrders _createOrders;
        private string cs;
        public PrintSalesOrderCCController(ISender mediator,IConfiguration _iconfig,ICreateOrders createOrders)
        {
            _mediator = mediator;
            Iconfig = _iconfig;
            _createOrders = createOrders;
        }
        //[HttpPost]
        //[Route("CreateKDSOrder")]
        //public async Task<ResponseModelWithClass> CreateIndolgeOrders(OrderHeader request)
        //{
        //    ResponseModelWithClass res = new ResponseModelWithClass();

        //    cs = Iconfig.GetConnectionString("CSKDS");

        //    IndolgeOrdersToKDS obj = new IndolgeOrdersToKDS(Iconfig);
        //    ResponseModelWithClass response = await obj.InsertIndolgeOrdersToKDS(request);
        //    //if ( res.HttpStatusCode == (int)HttpStatusCode.OK && res.MessageType == (int)MessageType.Success )
        //    //{

        //    //}
        //    return response;
        //}

        //[HttpPut]
        //[Route("UpdateKDSOrders")]
        //public async Task<ResponseModelWithClass> UpdateIndolgeOrders(string OrderId, List<Orders> request)
        //{
        //    ResponseModelWithClass res = new ResponseModelWithClass();

        //    cs = Iconfig.GetConnectionString("CSKDS");

        //    IndolgeOrdersToKDS obj = new IndolgeOrdersToKDS(Iconfig);
        //    ResponseModelWithClass response = await obj.UpdateIndolgeOrdersToKDS(OrderId, request);
        //    //if ( res.HttpStatusCode == (int)HttpStatusCode.OK && res.MessageType == (int)MessageType.Success )
        //    //{

        //    //}
        //    return response;
        //}


        //      [HttpPost]
        //      [Route("createSalesOrderCC")]
        //      public async Task<ResponseModelWithClass<CreateSalesOrderResponse>> CreateSalesOrderAsync(CreatePrintSalesOrderCCCommand request)
        //      {
        //          ResponseModelWithClass<CreateSalesOrderResponse> response = new ResponseModelWithClass<CreateSalesOrderResponse>();
        //          CreateSalesOrderResponse responseModel = new CreateSalesOrderResponse();
        //          try
        //          {
        //              PrintSalesOrderCC printSalesOrderCC = await _mediator.Send(request);


        //              responseModel.SalesID = printSalesOrderCC.SalesId;

        //              response.Result = responseModel;
        //              response.HttpStatusCode = (int)HttpStatusCode.OK;
        //              response.MessageType = (int)MessageType.Success;
        //              response.Message = "Sales Order created successfully.";

        //              return response;
        //          }
        //          catch (Exception ex)
        //          {

        //              response.Result = responseModel;
        //              response.HttpStatusCode = (int)HttpStatusCode.InternalServerError;
        //              response.MessageType = (int)MessageType.Success;
        //              response.Message = ex.Message + " | inner exception: " + ex.InnerException;

        //          }

        //          return response;
        //      }

        //      [HttpPost]
        //      [Route("createSalesOrderCC-88")]
        //      public async Task<ResponseModelWithClass<CreateSalesOrderResponse>> AddUSerType88Async(CreatePrintSalesOrderCCCommand request)
        //      {
        //          ResponseModelWithClass<CreateSalesOrderResponse> response = new ResponseModelWithClass<CreateSalesOrderResponse>();
        //          CreateSalesOrderResponse responseModel = new CreateSalesOrderResponse();
        //          try
        //          {
        //              PrintSalesOrderCC printSalesOrderCC = await _mediator.Send(request);

        //              responseModel.SalesID = printSalesOrderCC.SalesId;

        //              response.Result = responseModel;
        //              response.HttpStatusCode = (int)HttpStatusCode.OK;
        //              response.MessageType = (int)MessageType.Success;
        //              response.Message = "Sales Order created successfully.";

        //              return response;
        //          }
        //          catch (Exception ex)
        //          {

        //              response.Result = responseModel;
        //              response.HttpStatusCode = (int)HttpStatusCode.OK;
        //              response.MessageType = (int)MessageType.Success;
        //              response.Message = ex.Message + " | inner exception: " + ex.InnerException ;

        //          }

        //          return response;
        //      }
        //      // bilal khan
        //      [HttpPost]
        //      [Route("GetJsonObject")]
        //      public async Task<ResponseModelWithClass<CreateSalesOrderResponse>> CreateFoodDetails(FoodDetails request)
        //      {
        //          ResponseModelWithClass<CreateSalesOrderResponse> res = await _createOrders.InsertFoods(request);
        //          if ( res.HttpStatusCode == (int)HttpStatusCode.OK && res.MessageType == (int)MessageType.Success )
        //          {

        //          }
        //          return res;
        //} 

        //---------------------------------------------------------------------------------------

    }
}
