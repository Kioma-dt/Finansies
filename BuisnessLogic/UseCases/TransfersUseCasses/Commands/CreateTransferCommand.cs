using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BuisnessLogic.UseCases.TransfersUseCasses.Commands
{
    public sealed record CreateTransferCommand(Guid UserId,
        decimal Amount,
        string Description,
        DateTime Date,
        Guid FromAccountId,
        Guid ToAccountId)
        : IRequest;

    public class CreateTransferCommandHandler(ITransferRepository transferRepository,
        IAccountRepository accountRepository)
        : IRequestHandler<CreateTransferCommand>
    {
        public async Task Handle(CreateTransferCommand request, CancellationToken cancellationToken)
        {
            var fromAccount = await accountRepository.GetById(request.UserId, request.FromAccountId);
            var toAccount = await accountRepository.GetById(request.UserId, request.ToAccountId);

            if (fromAccount is null)
            {
                throw new ArgumentException($"No Account with Id: {request.FromAccountId}");
            }


            if (toAccount is null)
            {
                throw new ArgumentException($"No Account with Id: {request.ToAccountId}");
            }

            fromAccount.RemoveFromBalance(request.Amount);
            toAccount.AddToBalance(request.Amount);

            await transferRepository.Add(new Transfer
            {
                Amount = request.Amount,
                Description = request.Description,
                Date = request.Date,
                FromAccountId = request.FromAccountId,
                ToAccountId = request.ToAccountId,
                UserId = request.UserId
            });

            await accountRepository.Update(fromAccount);
            await accountRepository.Update(toAccount);
        }
    }
}
