using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BuisnessLogic.Shared;

namespace BuisnessLogic.UseCases.PlannedTransactionsUsesCasses.Commands
{
    public sealed record CreatePlannedTransactionCommand(Guid UserId,
            decimal Amount,
            string Description,
            TransactionType Type,
            DateTime StartDate,
            Guid? AccountId,
            Guid? CategoryId,
            Guid? FamilyMemberId,
            Guid? DebtId,
            uint NumberOfTransactions = 1,
            TransactionPeriodicity Periodicy = TransactionPeriodicity.Once)
        : IRequest;

    public class CreatePlannedTransactionCommandHandler(IPlannedTransactionRepository plannedTransactionRepository)
        : IRequestHandler<CreatePlannedTransactionCommand>
    {
        public async  Task Handle(CreatePlannedTransactionCommand request, CancellationToken cancellationToken)
        {
            var startDate = request.StartDate;
            for (int i = 0; i < request.NumberOfTransactions; i++)
            {
                var planedTransaction = new PlannedTransaction()
                {
                    Amount = request.Amount,
                    Description = request.Description,
                    Type = request.Type,
                    PlannedDate = startDate,
                    AccountId = request.AccountId,
                    CategoryId = request.CategoryId,
                    FamilyMemberId = request.FamilyMemberId,
                    DebtId = request.DebtId,
                    UserId = request.UserId
                };

                await plannedTransactionRepository.Add(planedTransaction);

                startDate = startDate.AddTransactionPeriodicity(request.Periodicy);
            }
        }
    }
}
