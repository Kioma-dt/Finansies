using BuisnessLogic.Entities;
using MediatR;
using BuisnessLogic.Repositories;

namespace BuisnessLogic.UseCases.PlannedTransactionsUsesCasses.Queries
{
    public sealed record GetPlannedTransactionIdQuery(Guid UserId, Guid Id)
        : IRequest<PlannedTransaction>;

    public class GetPlannedTransactionIdQueryHandler(IPlannedTransactionRepository plannedTransactionRepository)
        : IRequestHandler<GetPlannedTransactionIdQuery, PlannedTransaction>
    {
        public async Task<PlannedTransaction> Handle(GetPlannedTransactionIdQuery request, CancellationToken cancellationToken)
        {
            var plannedTransactions = await plannedTransactionRepository.GetById(request.UserId, request.Id);
            if (plannedTransactions is null)
            {
                throw new ArgumentException($"No Planned Transaction with Id: {request.Id}");
            }
            return plannedTransactions;
        }
    }
}
