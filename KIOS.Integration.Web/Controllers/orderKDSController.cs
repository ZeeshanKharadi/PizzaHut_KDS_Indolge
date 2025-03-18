using KIOS.Integration.Web.Model;
using KIOS.Integration.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KIOS.Integration.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class orderKDSController : ControllerBase
    {
        private readonly IConfiguration Iconfig;
        private string cs;
        public orderKDSController(IConfiguration iconfig)
        {
            Iconfig = iconfig;
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
        [Route("DeleteKDSOrder")]
        public async Task<ResponseModelWithClass> Delete(string OrderId)
        {
            ResponseModelWithClass res = new ResponseModelWithClass();
            cs = Iconfig.GetConnectionString("CSKDS");

            IndolgeOrdersToKDS obj = new IndolgeOrdersToKDS(Iconfig);
            ResponseModelWithClass response = await obj.DeleteOrderFromKDS(cs,OrderId);

            return response;
        }
    }
}
