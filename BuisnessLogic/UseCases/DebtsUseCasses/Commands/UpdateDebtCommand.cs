using BuisnessLogic.DebtInterestCalculator;
using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BuisnessLogic.UseCases.DebtsUseCasses.Commands
{
    public sealed record UpdateDebtCommand(Guid UserId,
        Guid DebtId,
        DateTime Date)
        : IRequest;

    public class UpdateDebtCommandHandler(IDebtRepository debtRepository,
        IDebtInterestCalculatorProvider debtInterestCalculatorProvider)
        : IRequestHandler<UpdateDebtCommand>
    {
        public async Task Handle(UpdateDebtCommand request, CancellationToken cancellationToken)
        {
            var debt = await debtRepository.GetById(request.UserId, request.DebtId);

            if (debt is null)
            {
                throw new ArgumentException($"No Debt with Id: {request.DebtId}");
            }

            //if (date > debt.EndDate)
            //{
            //    throw new Exception("The Debt Is Overdue!");
            //}

            var calculator = debtInterestCalculatorProvider.GetCalculator(debt.InterestType);

            var new_amount = calculator.Calculate(debt.StartAmount,
                                                  debt.InterestRate,
                                                  debt.CapitalisationsPerYear,
                                                  debt.FixedAddition,
                                                  debt.StartDate,
                                                  request.Date);

            var interests = new_amount - debt.TotalAmount;

            //interests = interests >= 0 ? interests : -interests;

            debt.ChargeInterest(interests);

            await debtRepository.Update(debt);
        }
    }
}
