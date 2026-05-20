using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using MediatR;

namespace BuisnessLogic.UseCases.PlannedTransactionsUsesCasses.Commands
{
    public sealed record AddTagToPlannedTransactionCommand(Guid UserId,
        Guid PlannedTransactionId,
        Guid TransactionTagId)
        : IRequest;

    public class AddTagToPlannedTransactionCommandHandler(IPlannedTransactionRepository plannedTransactionRepository,
        ITransactionTagRepository transactionTagRepository)
        : IRequestHandler<AddTagToPlannedTransactionCommand>
    {
        public async Task Handle(AddTagToPlannedTransactionCommand request, CancellationToken cancellationToken)
        {
            var plannedTransaction = await plannedTransactionRepository.GetById(request.UserId, request.PlannedTransactionId);

            if (plannedTransaction is null)
            {
                throw new ArgumentException($"No Planned Transaction with Id: {request.PlannedTransactionId}");
            }


            var transactionTag = await transactionTagRepository.GetById(request.UserId, request.TransactionTagId);

            if (transactionTag is null)
            {
                throw new ArgumentException($"No Transaction Tag with Id: {request.TransactionTagId}");
            }

            plannedTransaction.TransactionTags.Add(transactionTag);

            //transactionTag.PlannedTransactions.Add(plannedTransaction);

            await plannedTransactionRepository.Update(plannedTransaction);
        }
    }
}
