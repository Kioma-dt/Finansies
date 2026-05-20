using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogic.UseCases.AccountsUseCases.Commands
{

    public sealed record CreateAccountCommand(Guid UserId,
        string Name,
        decimal Balance,
        Guid? ParentId)
       : IRequest;

    public class CreateAccountCommandHandler(IAccountRepository accountRepository)
        : IRequestHandler<CreateAccountCommand>
    {
        public async Task Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            await accountRepository.Add(new Account()
            {
                Name = request.Name,
                Balance = request.Balance,
                ParentId = request.ParentId,
                UserId = request.UserId
            });
        }
    }
}
