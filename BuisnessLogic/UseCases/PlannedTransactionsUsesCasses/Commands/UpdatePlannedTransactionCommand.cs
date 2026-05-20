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
    public sealed record UpdatePlannedTransactionCommand(Guid UserId,
        Guid PlannedTransactionId,
        DateTime Date)
        : IRequest;

    public class UpdatePlannedTransactionCommandHandler(IPlannedTransactionRepository plannedTransactionRepository)
        : IRequestHandler<UpdatePlannedTransactionCommand>
    {
        public async Task Handle(UpdatePlannedTransactionCommand request, CancellationToken cancellationToken)
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
