using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace KIOS.Integration.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;

        public LoginController(IConfiguration config)
        {
            _config = config;
        }

        //[HttpPost]
        //public IActionResult Post()
        //{
        //    //your logic for login process
        //    //If login usrename and password are correct then proceed to generate token

        //    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        //    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        //    var Sectoken = new JwtSecurityToken(_config["Jwt:Issuer"],
        //      _config["Jwt:Issuer"],
        //      null,
        //      expires: DateTime.Now.AddHours(24),
        //      signingCredentials: credentials);

        //    var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);

        //    return Ok(token);
        //}

        /*

        [HttpGet]
        [Route("get-user-list")]
        public async Task<IList<UserResponse>> GetUserResponsesAsync()
        {
            return await _userService.GetUserList();
        }

        [HttpGet]
        [Route("user-by-email")]
        public async Task<IList<UserResponse>> GetUserResponsesByEmailAsync(string email)
        {
            return await _userService.GetUserByEmail(email);
        }

        [HttpPost]
        [Route("account-register")]
        public async Task<bool> AccountRegisterAsync(IList<AccountRegisterRequest> request)
        {
            return await _userService.AccountRegisterAsync(request); 
        }

        */
    }
}