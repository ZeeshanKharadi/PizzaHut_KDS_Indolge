using Microsoft.AspNetCore.Mvc;
using KIOS.Integration.Application.Services.Abstraction;

namespace KIOS.Integration.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MPOSStatusController : ControllerBase
    {
        private readonly ICheckPosStatusService _checkPosStatusService;

        public MPOSStatusController(ICheckPosStatusService checkPosStatusService)
        {
            _checkPosStatusService = checkPosStatusService;
        }
        
        /*
         //http://localhost:8082/api/MPOSStatus/check-pos-status?storeId=0072
        [HttpGet]
        [Route("check-pos-status")]
        public async Task<ResponseModelWithClass<CheckPOSStatusReposne>> CheckPosStatusAsync(string storeId)
        {
            return await _checkPosStatusService.CheckPosStatusAsync(storeId);
        }
        */
        
    }
}