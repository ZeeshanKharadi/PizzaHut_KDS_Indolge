using POS_Integration_CommonCore.Response;
using POS_IntegrationCommonDTO.Response;

namespace KIOS.Integration.Application.Services.Abstraction
{
    public interface ICheckPosStatusService
    {
        Task<ResponseModelWithClass<CheckPOSStatusReposne>> CheckPosStatusAsync(string storeId);
    }
}
