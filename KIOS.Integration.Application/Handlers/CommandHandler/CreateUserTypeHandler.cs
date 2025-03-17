using MediatR;
using KIOS.Integration.Application.Commands;
using POS_IntegrationCommonInfrastructure.Database;
using POS_IntegrationCommonInfrastructure.Model;

namespace KIOS.Integration.Application.Handlers.CommandHandler
{
    public class CreateUserTypeHandler : IRequestHandler<CreateUserTypeCommand, UserType>
    {
        private readonly AppDbContext _appDbContext;

        public CreateUserTypeHandler (AppDbContext appDbContext) 
        {
            _appDbContext = appDbContext;
        }

        public async Task<UserType> Handle(CreateUserTypeCommand request, CancellationToken cancellationToken)
        {
            UserType userType = new UserType 
            { 
                Name = request.Name, 
                CreatedOn = DateTime.Now,
                IsActive = true, 
                IsDeleted = false 
            };
            await _appDbContext.UserTypes.AddAsync(userType);
            await _appDbContext.SaveChangesAsync();
            return userType;
        }
    }
}
