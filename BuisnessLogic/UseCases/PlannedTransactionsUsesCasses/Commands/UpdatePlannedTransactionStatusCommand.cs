using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BuisnessLogic.UseCases.PlannedTransactionsUsesCasses.Commands
{
    public sealed record UpdatePlannedTransactionStatusCommand(Guid UserId,
        Guid PlannedTransactionId,
        DateTime Date)
        : IRequest;

    public class UpdatePlannedTransactionStatusCommandHandler(IPlannedTransactionRepository plannedTransactionRepository)
        : IRequestHandler<UpdatePlannedTransactionStatusCommand>
    {
        public async Task Handle(UpdatePlannedTransactionStatusCommand request, CancellationToken cancellationToken)
        {
            var plannedTransaction = await plannedTransactionRepository.GetById(request.UserId, request.PlannedTransactionId);

            if (plannedTransaction is null)
            {
                throw new ArgumentException($"No Planned Transaction with Id: {request.PlannedTransactionId}");
            }


            plannedTransaction.Update(request.Date);

            await plannedTransactionRepository.Update(plannedTransaction);
        }
    }
}
