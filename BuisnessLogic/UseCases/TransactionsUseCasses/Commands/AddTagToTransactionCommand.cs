using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using MediatR;

namespace BuisnessLogic.UseCases.TransactionsUseCasses.Commands
{
    public sealed record AddTagToTransactionCommand(Guid UserId,
        Guid TransactionId,
        Guid TransactionTagId)
        : IRequest;

    public class AddTagToTransactionCommandHandler(ITransactionRepository transactionRepository,
        ITransactionTagRepository transactionTagRepository)
        : IRequestHandler<AddTagToTransactionCommand>
    {
        public async Task Handle(AddTagToTransactionCommand request, CancellationToken cancellationToken)
        {
            var transaction = await transactionRepository.GetById(request.UserId, request.TransactionId);

            if (transaction is null)
            {
                throw new ArgumentException($"No Transaction with Id: {request.TransactionId}");
            }

            var transactionTag = await transactionTagRepository.GetById(request.UserId, request.TransactionTagId);

            if (transactionTag is null)
            {
                throw new ArgumentException($"No Transaction Tag with Id: {request.TransactionTagId}");
            }

            transaction.TransactionTags.Add(transactionTag);

            transactionTag.Transactions.Add(transaction);

            await transactionRepository.Update(transaction);
        }
    }
}
