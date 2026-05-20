using BuisnessLogic.DebtInterestCalculator;
using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogic.UseCases.PlannedTransactionsUsesCasses.Commands
{
    public sealed record PlanDebtPaymentsCommand(Guid UserId,
        Guid DebtId,
        TransactionPeriodicity Periodicity)
        : IRequest;

    public class PlanDebtPaymentsCommandHandler(IMediator mediator,
        IPlannedTransactionRepository plannedTransactionRepository,
        IDebtRepository debtRepository,
        IDebtInterestCalculatorProvider debtInterestCalculatorProvider)
        : IRequestHandler<PlanDebtPaymentsCommand>
    {
        public async Task Handle(PlanDebtPaymentsCommand request, CancellationToken cancellationToken)
        {
            var debt = await debtRepository.GetById(request.UserId, request.DebtId);

            if (debt is null)
            {
                throw new ArgumentException($"No Such Debt: {request.DebtId}");
            }

            var period = (decimal)(debt.EndDate - debt.StartDate).TotalDays;

            var totalAmount = debtInterestCalculatorProvider
                .GetCalculator(debt.InterestType)
                .Calculate
                (
                    debt.StartAmount,
                    debt.InterestRate,
                    debt.CapitalisationsPerYear,
                    debt.FixedAddition,
                    debt.StartDate,
                    debt.EndDate
                );

            var transactionAmount = request.Periodicity switch
            {
                TransactionPeriodicity.Daily => totalAmount / period,
                TransactionPeriodicity.Monthly => totalAmount / (period / 30m),
                TransactionPeriodicity.Yearly => totalAmount / (period / 365m),
                _ => totalAmount
            };

            var type = debt.Type switch
            {
                DebtType.Debit => TransactionType.Income,

                _ => TransactionType.Expense
            };

            var paymentsNumber = (uint)Math.Ceiling(totalAmount / transactionAmount);

            await mediator.Send(new CreatePlannedTransactionCommand(request.UserId,
                transactionAmount,
                debt.Name,
                type,
                debt.StartDate,                
                null,
                null,
                null,
                request.DebtId,
                paymentsNumber,
                request.Periodicity
                ));
        }
    }
}
