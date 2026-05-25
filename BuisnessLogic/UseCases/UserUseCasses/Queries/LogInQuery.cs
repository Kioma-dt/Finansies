using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BuisnessLogic.Entities;
using BuisnessLogic.AuthentificationService;

namespace BuisnessLogic.UseCases.UserUseCasses.Queries
{
    public sealed record LogInQuery(string Name, string Password)
        : IRequest<User?>
    { }

    public class LogInQueryHandler(IAuthentificationService authentificationService)
        : IRequestHandler<LogInQuery, User?>
    {
        public async Task<User?> Handle(LogInQuery request, CancellationToken cancellationToken)
        {
            var user = await authentificationService.LogIn(request.Name, request.Password);
            
            return user;
        }
    }
}
