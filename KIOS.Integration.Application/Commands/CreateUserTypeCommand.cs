using MediatR;
using POS_IntegrationCommonInfrastructure.Model;

namespace KIOS.Integration.Application.Commands
{

    public class CreateUserTypeCommand : IRequest<UserType>
    {
        public string Name { get; set; }
    }
}
