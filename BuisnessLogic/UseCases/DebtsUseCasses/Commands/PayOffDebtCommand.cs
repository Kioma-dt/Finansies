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
    public sealed record PayOffDebtCommand(Guid UserId,
        Guid DebtId,
        decimal Amount,
        DateTime Date)
        : IRequest;

    public class PayOffDebtCommandHandler(IDebtRepository debtRepository)
        : IRequestHandler<PayOffDebtCommand>
    {
        public async Task Handle(PayOffDebtCommand request, CancellationToken cancellationToken)
        {
            var debt = await debtRepository.GetById(request.UserId, request.DebtId);

            if (debt is null)
            {
                throw new ArgumentException($"No Debt with Id: {request.DebtId}");
            }

            debt.MakeAPayment(request.Amount, request.Date);

            await debtRepository.Update(debt);
        }
    }
}
