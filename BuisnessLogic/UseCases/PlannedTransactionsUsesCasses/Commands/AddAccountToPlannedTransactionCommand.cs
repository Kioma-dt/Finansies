using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogic.UseCases.PlannedTransactionsUsesCasses.Commands
{
    public sealed record AddAccountToPlannedTransactionCommand(Guid UserId,
        Guid PlannedTransactionId,
        Guid AccountId)
        : IRequest;

    public class AddAccountToPlannedTransactionCommandHandler(IPlannedTransactionRepository plannedTransactionRepository,
        IAccountRepository accountRepository)
        : IRequestHandler<AddAccountToPlannedTransactionCommand>
    {
        public async Task Handle(AddAccountToPlannedTransactionCommand request, CancellationToken cancellationToken)
        {
            var plannedTransaction = await plannedTransactionRepository.GetById(request.UserId, request.PlannedTransactionId);

            if (plannedTransaction is null)
            {
                throw new ArgumentException($"No Planned Transaction with Id: {request.PlannedTransactionId}");
            }

            var account = await accountRepository.GetById(request.UserId, request.AccountId);

            if (account is null)
            {
                throw new ArgumentException($"No Account with Id: {request.AccountId}");
            }


            plannedTransaction.AccountId = request.AccountId;

            //account.PlannedTransactions.Add(plannedTransaction);

            await plannedTransactionRepository.Update(plannedTransaction);
        }
    }
}
