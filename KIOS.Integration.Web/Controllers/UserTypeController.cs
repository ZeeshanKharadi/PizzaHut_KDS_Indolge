using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KIOS.Integration.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTypeController : ControllerBase
    {
        private readonly ISender _mediator;

        public UserTypeController(ISender mediator)
        {
            _mediator = mediator;
        }

        /*

        [HttpGet]
        [Route("get-all-userTypes")]
        public async Task<IList<UserTypeResponse>> GetAll()
        {
            return await _mediator.Send(new GetAllUSerTypeQuery());
        }

        [HttpPost]
        [Route("create-userTypes")]
        public async Task<UserType> AddUSerTypeAsync(CreateUserTypeCommand request)
        {
           UserType users = await _mediator.Send(request);
           return users;
        }

        */
    }
}
