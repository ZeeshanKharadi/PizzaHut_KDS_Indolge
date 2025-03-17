using POS_IntegrationCommonDTO.Request;
using POS_IntegrationCommonDTO.Response;

namespace KIOS.Integration.Application.Services.Abstraction
{
    public interface IUserService
    {
        Task<IList<UserResponse>> GetUserList();
        Task<IList<UserResponse>> GetUserByEmail(string email);
        Task<bool> AccountRegisterAsync(IList<AccountRegisterRequest> request);
    }
}









