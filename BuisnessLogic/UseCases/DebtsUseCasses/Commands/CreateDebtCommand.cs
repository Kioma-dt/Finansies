using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.Repositories;
using BuisnessLogic.UseCases.PlannedTransactionsUsesCasses.Commands;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogic.UseCases.DebtsUseCasses.Commands
{
    public sealed record CreateDebtCommand(Guid UserId,
        string Name,
        decimal Amount,
        DebtType Type,
        DateTime StartDate,
        DateTime EndDate,
        Guid? CategoryId,
        Guid? FamilyMemberId,
        decimal CapitalisatonsPerYear,
        InterestType InterestType,
        decimal InterestRate,
        decimal FixedAddition,
        bool IsAutoPlanned,
        TransactionPeriodicity TransactionPeriodicity)
        : IRequest;

    public class CreateDebtCommandHandler(IMediator mediator,
        IDebtRepository debtRepository)
        : IRequestHandler<CreateDebtCommand>
    {
        public async Task Handle(CreateDebtCommand request, CancellationToken cancellationToken)
        {
            var id = Guid.NewGuid();
            var debt = new Debt()
            {
                Id = id,
                Name = request.Name,
                StartAmount = request.Amount,
                TotalAmount = request.Amount,
                PaidAmount = 0,
                InterestRate = request.InterestRate,
                CapitalisationsPerYear = request.CapitalisatonsPerYear,
                FixedAddition = request.FixedAddition,
                Type = request.Type,
                InterestType = request.InterestType,
                StartDate = request.StartDate,
                LastPaidDate = request.StartDate,
                EndDate = request.EndDate,
                CategoryId = request.CategoryId,
                FamilyMemberId = request.FamilyMemberId,
                UserId = request.UserId
            };

            await debtRepository.Add(debt);

            if (request.IsAutoPlanned)
            {
                await mediator.Send(new PlanDebtPaymentsCommand(request.UserId, id, request.TransactionPeriodicity));
            }
        }
    }
}
