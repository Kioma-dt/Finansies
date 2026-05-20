using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.Repositories;
using BuisnessLogic.UseCases.TransactionsUseCasses.Commands;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BuisnessLogic.UseCases.PlannedTransactionsUsesCasses.Commands
{
    public sealed record ConfirmPlannedTransactionCommand(Guid UserId,
        Guid PlannedTransactionId,
        DateTime? Date = null)
        : IRequest;

    public class ConfirmPlannedTransactionCommandHandler(IMediator mediator,
        IPlannedTransactionRepository plannedTransactionRepository)
        : IRequestHandler<ConfirmPlannedTransactionCommand>
    {
        public async Task Handle(ConfirmPlannedTransactionCommand request, CancellationToken cancellationToken)
        {
            var plannedTransaction = await plannedTransactionRepository.GetById(request.UserId, request.PlannedTransactionId);

            if (plannedTransaction is null)
            {
                throw new ArgumentException($"No Planned Transaction with Id: {request.PlannedTransactionId}");
            }

            if (plannedTransaction.Status == PlannedTransactionStatus.Confirmed)
            {
                throw new ArgumentException($"No Planned Transaction is Already Confirmed: {plannedTransaction.Description}");
            }


            if (plannedTransaction.AccountId is null)
            {
                throw new ArgumentException("Planned Transaction Does't Contain Account");
            }

            var date = request.Date;
            if (date is null)
            {
                date = plannedTransaction.PlannedDate;
            }

            await mediator.Send(new CreateTransactionCommand(
                plannedTransaction.UserId,
                plannedTransaction.Amount,
                plannedTransaction.Description,
                date.Value,
                plannedTransaction.Type,
                plannedTransaction.AccountId.Value,
                plannedTransaction.CategoryId,
                plannedTransaction.FamilyMemberId,
                plannedTransaction.DebtId
            ));

            plannedTransaction.Conirm();
            await plannedTransactionRepository.Update(plannedTransaction);
        }
    }
}
