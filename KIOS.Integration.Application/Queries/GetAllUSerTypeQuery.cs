using MediatR;
using POS_IntegrationCommonDTO.Response;

namespace KIOS.Integration.Application.Queries
{
    public class GetAllUSerTypeQuery : IRequest<List<UserTypeResponse>>
    {
        public string Name { get; set; }
        public long Id { get; set; }
    }
}
