using Microsoft.AspNetCore.Mvc;
using KIOS.Integration.Application.Services.Abstraction;

namespace KIOS.Integration.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KIOSOrderController : ControllerBase
    {
        private readonly ILogger<KIOSOrderController> _logger;
        private readonly ICreateOrderService _createOrderService;

        public KIOSOrderController(ILogger<KIOSOrderController> logger, ICreateOrderService createOrderService)
        {
            _logger = logger;
            _createOrderService = createOrderService;
        }

        /*
        [HttpPost]
        [Route("create-order-pos")]
        public async Task<ResponseModelWithClass<CreateOrderResponse>> CreateOrderKFC(KIOS.Integration.Application.Commands.CreateRetailTransactionCommand request)
        {
         
            return await _createOrderService.CreateOrderKFC(request);
        }
        */
    }
}