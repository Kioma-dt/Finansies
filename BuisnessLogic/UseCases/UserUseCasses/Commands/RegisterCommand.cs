using MediatR;
using BuisnessLogic.AuthentificationService;
using BuisnessLogic.Repositories;
using BuisnessLogic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogic.UseCases.UserUseCasses.Commands
{
    public sealed record RegisterCommand(string Name, string Password)
        : IRequest<User?>;

    public class RegisterCommandHandler(IAuthentificationService authentificationService,
        IUserRepository userRepository)
        : IRequestHandler<RegisterCommand, User?>
    {
        public async Task<User?> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var user = await authentificationService.Register(request.Name, request.Password);

            if (user is not null)
            {
                await userRepository.Add(user);
            }

            return user;
        }
    }
}
