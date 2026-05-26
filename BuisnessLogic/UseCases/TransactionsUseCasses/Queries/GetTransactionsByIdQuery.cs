using BuisnessLogic.Entities;
using MediatR;
using BuisnessLogic.Repositories;

namespace BuisnessLogic.UseCases.TransactionsUseCasses.Queries
{
    public sealed record GetTransactionsByIdQuery(Guid UserId, Guid Id)
        : IRequest<Transaction>;

    public class GetTransactionsByIdQueryHandler(ITransactionRepository transactionRepository)
        : IRequestHandler<GetTransactionsByIdQuery, Transaction>
    {
        public async Task<Transaction> Handle(GetTransactionsByIdQuery request, CancellationToken cancellationToken)
        {
            var transaction = await transactionRepository.GetById(request.UserId, request.Id);
            if (transaction is null)
            {
                throw new ArgumentException($"No Transaction with Id: {request.Id}");
            }
            return transaction;
        }
    }
}
